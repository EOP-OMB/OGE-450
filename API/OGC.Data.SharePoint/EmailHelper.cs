using System.Collections.Generic;
using OGC.Data.SharePoint.Models;
using OMB.SharePoint.Infrastructure;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Specialized;
using Microsoft.SharePoint.Client.Utilities;
using System.Net.Mail;
using System.ComponentModel;
using System.Linq;

namespace OGC.Data.SharePoint
{
    public class EmailHelper
    {
        public static Notifications GetEmail(string notificationType, UserInfo user, Dictionary<string, string> dictionary)
        {
            var template = NotificationTemplates.GetBy("Title", notificationType);

            return GetEmail(template, user, dictionary);
        }

        public static Notifications GetEmail(NotificationTemplates template, UserInfo user, Dictionary<string, string> dictionary)
        {
            var body = Replace(template.Body, dictionary);

            return GetEmail(template, user, dictionary, body);
        }

        public static Notifications GetEmail(string notificationType, UserInfo user, Dictionary<string, string> dictionary, string body)
        {
            var template = NotificationTemplates.GetBy("Title", notificationType);

            if (string.IsNullOrEmpty(body))
                body = Replace(template.Body, dictionary);

            return GetEmail(template, user, dictionary, body);
        }

        public static Notifications GetEmail(NotificationTemplates template, UserInfo user, Dictionary<string, string> dictionary, string body)
        {
            var notification = new Notifications();

            notification.Status = "Pending";
            notification.Title = template.Title;
            notification.Subject = Replace(template.Subject, dictionary);
            notification.Body = body;
            notification.Application = template.Application;
            notification.IsAnnouncment = template.Frequency != "Real Time";

            if (template.RecipientType == "User")
                notification.Recipient = user.Email;
            else if (template.RecipientType == "Group")
            {
                // If the group is email enabled, go ahead and just email the DL, otherwise, get each user in the groups email.
                if (SharePointHelper.EmailSharePointGroup)
                    notification.Recipient = template.RecipientColumn + "@omb.gov";
                else
                    notification.Recipient = GetGroupEmails(template.RecipientColumn);
            }

            if (template.IncludeCc)
                notification.Cc = dictionary["Cc"];

            return notification;
        }

        public static void AddEmail(string notificationType, UserInfo user, Dictionary<string, string> dictionary)
        {
            AddEmail(GetEmail(notificationType, user, dictionary));
        }

        public static void AddEmail(Notifications notification)
        {
            notification.Save();
        }

        private static string Replace(string text, Dictionary<string, string> dictionary)
        {
            foreach (KeyValuePair<string, string> entry in dictionary)
                text = text.Replace("[" + entry.Key + "]", entry.Value);

            return text;
        }

        private static string GetGroupEmails(string groupName)
        {
            var users = EmailHelper.GetUsersInGroup(groupName);
            var emails = "";

            foreach (User u in users)
            {
                if (!string.IsNullOrEmpty(u.Email))
                    emails += u.Email + ",";
            }
            
            emails = emails.TrimEnd(',');

            return emails;
        }

        public static UserCollection GetUsersInGroup(string groupName)
        {
            using (ClientContext ctx = new ClientContext(SharePointHelper.Url))
            {
                var web = SharePointHelper.GetWeb(ctx);

                var group = SharePointHelper.GetGroup(ctx, web, groupName);
                var users = group.Users;

                ctx.Load(users);
                ctx.ExecuteQuery();

                return users;
            }
        }

        public static string GetEmailText(string notificationType, Dictionary<string, string> dictionary)
        {
            var template = NotificationTemplates.GetBy("Title", notificationType);

            return Replace(template.Body, dictionary);
        }

        public static void ProcessEmails()
        {
            GenerateNewEmails();
            SendEmail();
        }

        private static void GenerateNewEmails()
        {
            // get all templates that are enabled and not real time
            var templates = NotificationTemplates.GetAll().Where(x => x.Enabled == true && x.Frequency != "Real Time").ToList();

            foreach (NotificationTemplates template in templates)
            {
                // Check for next run date
                if ((template.NextRunDate ?? DateTime.MaxValue) <= DateTime.UtcNow)
                    GenerateEmailsFromTemplate(template);
            }
        }

        private static void GenerateEmailsFromTemplate(NotificationTemplates template)
        {
            var status = "Success";

            try
            {
                // Based on List, generate emails, currently only OGEForm450 templates are available
                if (template.SharePointList == "OGEForm450")
                    GenerateEmailsForOGEForm450(template);
            }
            catch (Exception ex)
            {
                status = "Error: " + ex.Message;
            }
            finally
            {
                // This may seem a little weird, why not just add frequency to DateTime.Now?
                // Well, eventually that would get skewed if this process takes any time to run.  We want the time of day to be consistent.
                // So if Next Run Date is set to Midnight, it will always be at midnight.
                // The loop is basically just here in case it gets stuck or set back way in the past.  
                // The loop ensures that the next run date that is set will be in the future.

                // Update Next Run Date
                while (template.NextRunDate < DateTime.UtcNow)
                    template.NextRunDate = GetNextRunDate((DateTime)template.NextRunDate, template.Frequency);

                template.LastRunDate = DateTime.UtcNow;
                template.LastRunStatus = status;

                template.Save();
            }
        }

        private static DateTime? GetNextRunDate(DateTime nextRunDate, string frequency)
        {
            switch (frequency)
            {
                case "Daily":
                    nextRunDate = nextRunDate.AddDays(1);
                    break;
                case "Weekly":
                    nextRunDate = nextRunDate.AddDays(7);
                    break;
                case "Monthly":
                    nextRunDate = nextRunDate.AddMonths(1);
                    break;
            }

            return nextRunDate;
        }

        private static void GenerateEmailsForOGEForm450(NotificationTemplates template)
        {
            var items = OGEForm450.GetAllByView(template.ViewName);

            foreach (OGEForm450 item in items)
            {
                var user = UserInfo.GetUser(item.Filer);
                var email = GetEmail(template, user, item.GetEmailData(user));

                EmailHelper.AddEmail(email);
            }
        }

        private static void SendEmail()
        {
            var emails = Notifications.GetAllBy("Status", "Pending");

            foreach (Notifications email in emails)
            {
                try
                {
                    SendEmail(email);
                }
                catch (Exception ex)
                {
                    email.Status = "Error";
                    email.SentDateTime = null;
                    email.ErrorMessage = ex.Message;

                    email.Save();
                }
            }
        }

        private static void SendEmail(Notifications email)
        {
            var client = new SmtpClient("SMTP URL");
 
            var from = new MailAddress("", "Ethics App", System.Text.Encoding.UTF8);

            var message = new MailMessage();

            try
            {
                message.From = from;

                foreach (string to in email.Recipient.Split(','))
                    message.To.Add(new MailAddress(to));

                message.ReplyToList.Add(new MailAddress("DONOTREPLY@"));
                message.IsBodyHtml = true;

                message.Body = email.Body;

                if (!string.IsNullOrEmpty(email.Cc))
                {
                    foreach (string cc in email.Cc.Split(',', ';'))
                        message.CC.Add(new MailAddress(cc));
                }   

                message.Subject = email.Subject;
                message.SubjectEncoding = System.Text.Encoding.UTF8;

                //if (email.Application == Constants.ApplicationName.EVENT_CLEARANCE)
                //{
                //    AddAttachments(message);
                //}
                

                client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

                client.Send(message);

                email.Status = "Sent";
                email.SentDateTime = DateTime.Now;
                email.ErrorMessage = "Email sent successfully.";
            }
            catch (Exception ex)
            {
                email.Status = "Error";
                email.SentDateTime = null;
                email.ErrorMessage = ex.Message;
            }
            finally
            {
                message.Dispose();
                email.Save();
            }
        }

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            int id = (int)e.UserState;
            var email = Notifications.Get(id);

            if (e.Cancelled)
            {
                email.Status = "Pending";
                email.SentDateTime = null;
                email.ErrorMessage = "";
            }
            if (e.Error != null)
            {
                email.Status = "Error";
                email.SentDateTime = null;
                email.ErrorMessage = e.Error.Message;
            }
            else
            {
                email.Status = "Sent";
                email.SentDateTime = DateTime.Now;
                email.ErrorMessage = "Email sent successfully.";
            }

            email.Save();
        }
    }
}