using System;
using System.Configuration;
using Microsoft.SharePoint.Client;
using OMB.SharePoint.Infrastructure;
using System.Collections.Generic;
using System.IO;

namespace OGC.Data.SharePoint.Models
{
    public class EthicsForm : SPListBase<EthicsForm>, ISPList
    {
        #region Properties
        public string Description { get; set; }
   
        public string FileName { get; set; }

        public int Size { get; set; }
        public string FormType { get; set; }
        public byte[] Content { get; set; }

        public string ContentType { get; set; }

        public int SortOrder { get; set; }
        #endregion

        public EthicsForm()
        {
            this.ListName = "Ethics Forms";
        }

        #region Mapping
        public override void MapFromList(ListItem item, bool includeChildren = false)
        {
            base.MapFromList(item);

            //FileName = SharePointHelper.ToStringNullSafe(item["Name"]);
            Description = SharePointHelper.ToStringNullSafe(item["Description0"]);
            FormType = SharePointHelper.ToStringNullSafe(item["FormType"]);
            //ContentType = SharePointHelper.ToStringNullSafe(item["ContentType"]);
            SortOrder = Convert.ToInt32(item["SortOrder"]);
            //Size = Convert.ToInt32(item["Size"]);
        }
        #endregion

        new public static EthicsForm Get(int id)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            EthicsForm t = new EthicsForm();

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
                    t.FileName = item.File.Name;
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
    }
}
