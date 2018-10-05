using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.IO;

namespace OGC.Data.SharePoint.Models
{
    public class Attachment : SPListBase<Attachment>, ISPList
    {
        #region Properties
        public int EventRequestId { get; set; }
        public string FileName { get; set; }

        public int Size { get; set; }
        public string TypeOfAttachment { get; set; }
        public string AttachmentGuid { get; set; }
        public byte[] Content { get; set; }
        #endregion

        public Attachment()
        {
            this.ListName = "Attachments";
        }

        #region Mapping
        public override void MapToList(ListItem dest)
        {
            Title = FileName;

            base.MapToList(dest);

            if (EventRequestId > 0)
            {
                var flv = new FieldLookupValue();
                flv.LookupId = EventRequestId;

                dest["EventRequestId"] = flv;
            }

            dest["AttachmentGuid"] = AttachmentGuid.ToString();
            dest["TypeOfAttachment"] = TypeOfAttachment;
            dest["Size"] = Size;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            if ((FieldLookupValue)item["EventRequestId"] != null)
            {
                EventRequestId = ((FieldLookupValue)item["EventRequestId"]).LookupId;
            }

            FileName = Title;
            Size = Convert.ToInt32(item["Size"]);
            AttachmentGuid = SharePointHelper.ToStringNullSafe(item["AttachmentGuid"]);
            TypeOfAttachment = SharePointHelper.ToStringNullSafe(item["TypeOfAttachment"]);
        }
        #endregion

        new public static Attachment Get(int id)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            Attachment t = new Attachment();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName);

                var item = list.GetItemById(id);

                ctx.Load(item);
                ctx.Load(item.File);
                ctx.ExecuteQuery();

                t.MapFromList(item, true);

                using (var memory = new MemoryStream())
                {
                    var data = item.File.OpenBinaryStream();
                    ctx.Load(item.File);
                    ctx.ExecuteQuery();

                    data.Value.CopyTo(memory);

                    t.Content = memory.ToArray();
                }

                //var byteCount = fileInformation.Stream.Read(t.Content, 0, t.Size);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + t.ListName + " with id " + id + ". " + ex.Message);
            }

            return t;

        }

        public bool Create()
        {
            bool returnVal = false;

            try
            {
                SPContext = new ClientContext(SharePointHelper.Url);

                var web = SharePointHelper.GetWeb(SPContext);
                var list = SharePointHelper.GetList(SPContext, web, ListName);

                Folder folder = GetFolderIfExists(list, AttachmentGuid.ToString());

                if (folder == null)
                    folder = CreateFolder(list, AttachmentGuid.ToString());

                FileCreationInformation fci = new FileCreationInformation();

                fci.ContentStream = new MemoryStream(Content);
                fci.Url = FileName;
                fci.Overwrite = true;

                Microsoft.SharePoint.Client.File fileToUpload = folder.Files.Add(fci);

                MapToList(fileToUpload.ListItemAllFields);
                fileToUpload.ListItemAllFields.Update();

                SPContext.Load(fileToUpload);
                SPContext.Load(fileToUpload.ListItemAllFields);

                SPContext.ExecuteQuery();

                returnVal = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to save " + ListName + " with id " + Id + ". " + ex.Message);
            }

            return returnVal;
        }

        private Folder GetFolderIfExists(List list, string folder)
        {
            FolderCollection folders = list.RootFolder.Folders;

            SPContext.Load(folders, fl => fl.Include(ct => ct.Name).Where(ct => ct.Name == folder));

            SPContext.ExecuteQuery();

            return folders.FirstOrDefault();
        }

        private Folder CreateFolder(List list, string folderName)
        {
            ListItem newItem = null; 

            try
            {
                ListItemCreationInformation info = new ListItemCreationInformation();

                info.UnderlyingObjectType = FileSystemObjectType.Folder;
                info.LeafName = folderName.Trim();//Trim for spaces.Just extra check

                newItem = list.AddItem(info);

                newItem["Title"] = folderName;
                newItem.Update();

                SPContext.ExecuteQuery();
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }

            return newItem == null ? null : newItem.Folder;
        }
    }
}
