using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.IO;

namespace OGC.Data.SharePoint.Models
{
    public class OMBEthicsAttachments : SPListBase<OMBEthicsAttachments>, ISPList
    {
        #region Properties
        public string FileName { get; set; }
        public string AssociatedEthicsFormId { get; set; }
        public byte[] Content { get; set; }
        #endregion

        public OMBEthicsAttachments()
        {
            this.ListName = "OMB Ethics Attachments Library";
        }

        #region Mapping
        public override void MapToList(ListItem dest)
        {
            Title = FileName;

            base.MapToList(dest);

            dest["Associated_x0020_Ethics_x0020_Form_x0020_ID"] = AssociatedEthicsFormId;
        }

        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            AssociatedEthicsFormId = SharePointHelper.ToStringNullSafe(item["Associated_x0020_Ethics_x0020_Form_x0020_ID"]);
            FileName = Title;
        }
        #endregion

        new public static OMBEthicsAttachments Get(int id)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            OMBEthicsAttachments t = new OMBEthicsAttachments();

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
