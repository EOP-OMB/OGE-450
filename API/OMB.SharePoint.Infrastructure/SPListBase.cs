using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OMB.SharePoint.Infrastructure
{
    public abstract class SPListBase<T> : IDisposable where T : ISPList, new()
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

        public virtual T Save()
        {
            ListItem newItem;

            try
            {
                SPContext = new ClientContext(SharePointHelper.Url);

                var web = SharePointHelper.GetWeb(SPContext);
                var list = SharePointHelper.GetList(SPContext, web, ListName);

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

            return Get(newItem.Id);
        }

        public void Delete()
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            T t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName);

                var item = list.GetItemById(Id);
                item.DeleteObject();
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete " + t.ListName + " with id " + Id + ". " + ex.Message, ex);
            }
        }

        public static T Get(int id)
        {
            var ctx = new ClientContext(SharePointHelper.Url);

            T t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName);

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

        public static List<T> GetAllByView(string viewName)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);
                
                var view = list.Views.GetByTitle(viewName);
                ctx.Load(view);
                ctx.ExecuteQuery();

                var caml = new CamlQuery();
                caml.ViewXml = string.Format("<View><Query>{0}</Query></View>", view.ViewQuery);

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

        public static List<T> GetAll()
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = SharePointHelper.GetAllCaml();
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

        public static List<T> GetAllJoined(string joinList, string joinField, string[] projectedFields)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = SharePointHelper.GetAllJoinCaml(joinList, joinField, projectedFields);
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

        public static T GetBy(string key, string value)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName);

                var caml = SharePointHelper.GetByCaml(key, value);
                var items = list.GetItems(caml);
                ctx.Load(items);
                ctx.ExecuteQuery();

                var item = items.FirstOrDefault();

                t.MapFromList(item);
               
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + t.ListName + ". " + ex.Message, ex);
            }

            return t;
        }

        public static T GetBy(string key, int value)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var t = new T();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, t.ListName);

                var caml = SharePointHelper.GetByCaml(key, value);
                var items = list.GetItems(caml);
                ctx.Load(items);
                ctx.ExecuteQuery();

                var item = items.FirstOrDefault();

                t.MapFromList(item);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to get " + t.ListName + ". " + ex.Message, ex);
            }

            return t;
        }

        public static List<T> GetAllByUser(string key, int userId)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();

            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = SharePointHelper.GetByUserCaml(key, userId);
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

        public static List<T> GetAllBy(string key, int id, bool notEqual = false)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = SharePointHelper.GetByCaml(key, id, notEqual);
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

        public static List<T> GetAllBy(string key, string value, bool notEqual = false)
        {
            var ctx = new ClientContext(SharePointHelper.Url);
            var type = new T();
            var results = new List<T>();

            try
            {
                var web = SharePointHelper.GetWeb(ctx);
                var list = SharePointHelper.GetList(ctx, web, type.ListName);

                var caml = SharePointHelper.GetByCaml(key, value, notEqual);
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
