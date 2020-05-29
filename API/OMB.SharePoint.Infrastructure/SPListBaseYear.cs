using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OMB.SharePoint.Infrastructure
{
    public abstract class SPListBaseYear<T> : IDisposable where T : ISPList, new()
    {
        [NonSerialized]
        public ClientContext SPContext;

        public void Dispose()
        {
            if (SPContext != null)
                SPContext.Dispose();

            this.Dispose();
        }

        public string ListName { get; set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; }

        public virtual void MapFromList(ListItem item, bool includeChildren = false)
        {
            this.Id = item.Id;
            this.Title = item["Title"] == null ? "No Title" : item["Title"].ToString();

            this.CreatedBy = ((FieldUserValue)item["Author"]).LookupValue;
            this.Created = Convert.ToDateTime(item["Created"]);
            this.ModifiedBy = ((FieldUserValue)item["Editor"]).LookupValue;
            this.Modified = Convert.ToDateTime(item["Modified"]);
        }

        public virtual void MapToList(ListItem dest)
        {
            dest["Title"] = string.IsNullOrEmpty(Title) ? "No Title" : Title;
        }

        public Dictionary<string, string> GetFieldVersionHistory(string fieldName)
        {
            var dict = new Dictionary<string, string>();

            using (ClientContext ctx = new ClientContext(SharePointHelper.Url))
            {
                // Get versions of specific document from library - item and file
                List list = ctx.Web.Lists.GetByTitle(ListName);
                ListItem item = list.GetItemById(Id);
                ListItemVersionCollection versions = item.Versions;
                ctx.Load(list);
                ctx.Load(item);
                ctx.Load(versions);

                ctx.ExecuteQuery();

                // Loop list item versions and access data of specific fields
                foreach (var version in versions)
                {
                    string versionValue = string.Empty;
                    if (version.FieldValues[fieldName] != null)
                    {
                        versionValue = version.FieldValues[fieldName].ToString();
                    }

                    dict.Add(version.VersionLabel, versionValue);
                }
            }

            return dict;
        }

        public virtual T Save(int year)
        {
            ListItem newItem;

            try
            {
                SPContext = new ClientContext(SharePointHelper.Url);

                var web = SharePointHelper.GetWeb(SPContext);
                var list = SharePointHelper.GetList(SPContext, web, ListName + year.ToString());

                if (Id == 0)
                {
                    // Create
                    var itemCreateInfo = new ListItemCreationInformation();
                    newItem = list.AddItem(itemCreateInfo);
                }
                else
                {
                    // Update
                    newItem = list.GetItemById(Id);
                }

                MapToList(newItem);
                newItem.Update();
                SPContext.Load(newItem);
                SPContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save " + ListName + " with id " + Id + ". " + ex.Message, ex);
            }

            return Get(newItem.Id, year);
        }

        public static T Get(int id, int year)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            T t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName + year.ToString());

                var item = list.GetItemById(id);
                ctx.Load(item);
                ctx.ExecuteQuery();

                t.MapFromList(item, true);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + t.ListName + " with id " + id + ". " + ex.Message, ex);
            }

            return t;
        }

        public void Delete(int year)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            T t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName + year.ToString());

                var item = list.GetItemById(Id);
                item.DeleteObject();
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete " + t.ListName + " with id " + Id + ". " + ex.Message, ex);
            }
        }

        public new static List<T> GetAllBy(string key, int id, int year)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName + year.ToString());

                var caml = SharePointHelper.GetByCaml(key, id, false);
                var items = list.GetItems(caml);
                ctx.Load(items);
                ctx.ExecuteQuery();

                foreach (ListItem item in items)
                {
                    var t = new T();

                    t.MapFromList(item);
                    results.Add(t);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + type.ListName + ". " + ex.Message, ex);
            }

            return results;
        }
    }
}
