using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace ServiceManager
{
    public class SharepointRepository
    {
        private const string Citizen = "شهروند";
        private const string Employee = "کارمند";
        private const string Individual = "فردی";
        private const string Group = "گروهی";

        private ClientContext myContext = null;// new ClientContext();
        private List MyList = null;
        private ProjectEntity Project5 = new ProjectEntity();
        public SharepointRepository()
        {
            try
            {
                Project5 = LoadPanelProjectData(5);
                myContext =
                            new ClientContext(
                                Project5.ProjectUrl)
                            {
                                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                            };
                Web myWeb = myContext.Web;
                MyList = myWeb.Lists.GetByTitle(Project5.ProjectLists.SingleOrDefault(pl => pl.ListId == 40).ListTitle);
            }
            catch(Exception ex)
            { }
        }
        private string SetupQueryExpression(string fieldName, string fieldValue, string dataType, string condition,
            string extraValueCondition = "", string extraFieldNameCondition = "")
        {
            string query = @"<{0}>
                                 <FieldRef Name='{1}' {5}/><Value Type='{2}' {4}>{3}</Value>
                             </{0}>";
            return string.Format(query, condition, fieldName, dataType, fieldValue, extraValueCondition, extraFieldNameCondition);
        }
        private ProjectEntity LoadPanelProjectData(int projectId)
        {
            try
            {
                var context =
                    new ClientContext("http://eportal.qom.ir:8080/WebServiceAdminPanel/")
                    {
                        Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                    };
                var projectList = context.Web.Lists.GetByTitle(@"Projects");
                var camlQuery = new CamlQuery
                {
                    ViewXml = string.Empty
                };
                var collListItem = projectList.GetItemById(projectId);
                context.Load(collListItem);
                context.ExecuteQuery();

                var project = new ProjectEntity();
                foreach (KeyValuePair<string, object> data in collListItem.FieldValues)
                {
                    var key = data.Key;
                    switch (key)
                    {
                        case "ID":
                            project.ProjectId = int.Parse(data.Value.ToString());
                            break;
                        case "Title":
                            project.ProjectTitle = data.Value.ToString();
                            break;
                        case "ProjectDescription":
                            project.ProjectDescription = string.IsNullOrEmpty((string)data.Value) ? null : data.Value.ToString();
                            break;
                        case "ProjectUrl":
                            project.ProjectUrl = data.Value.ToString();
                            break;
                    }
                }
                if (project.ProjectId == 0) return null;


                var announcementsList = context.Web.Lists.GetByTitle(@"ProjectLists");
                camlQuery = new CamlQuery
                {
                    ViewXml =
                        "<View><Query><Where><Eq><FieldRef Name='ProjectId' /><Value Type='Integer'>" + projectId +
                        "</Value></Eq></Where></Query></view>"
                };
                var collListItems = announcementsList.GetItems(camlQuery);
                context.Load(collListItems);
                context.ExecuteQuery();
                var lists = new List<ListEntity>();
                foreach (Microsoft.SharePoint.Client.ListItem oListItem in collListItems)
                {
                    var list = new ListEntity { ProjectId = project.ProjectId };
                    foreach (KeyValuePair<string, object> data in oListItem.FieldValues)
                    {
                        var key = data.Key;
                        switch (key)
                        {
                            case "ID":
                                list.ListId = int.Parse(data.Value.ToString());
                                break;
                            case "Title":
                                list.ListTitle = data.Value.ToString();
                                break;
                            case "ListDescription":
                                list.ListDescrption = string.IsNullOrEmpty((string)data.Value) ? null : data.Value.ToString();
                                break;
                        }
                    }
                    if (list.ListId == 0) continue;

                    var announcementsFieldsList = context.Web.Lists.GetByTitle(@"ListFields");
                    var fieldCamlQuery = new CamlQuery
                    {
                        ViewXml =
                            "<View><Query><Where><Eq><FieldRef Name='ListId' /><Value Type='Integer'>" + list.ListId +
                            "</Value></Eq></Where></Query></view>"
                    };
                    var fieldsCollListItem = announcementsFieldsList.GetItems(fieldCamlQuery);
                    context.Load(fieldsCollListItem);
                    context.ExecuteQuery();
                    var fields = new List<FieldEntity>();
                    foreach (Microsoft.SharePoint.Client.ListItem oFiledsListItem in fieldsCollListItem)
                    {
                        var field = new FieldEntity { ListId = list.ListId };
                        foreach (KeyValuePair<string, object> fieldData in oFiledsListItem.FieldValues)
                        {
                            var key = fieldData.Key;
                            switch (key)
                            {
                                case "ID":
                                    field.FieldId = int.Parse(fieldData.Value.ToString());
                                    break;
                                case "InternalName":
                                    field.FieldTitle = fieldData.Value.ToString();
                                    break;
                                case "FieldDescription":
                                    field.FieldDescription = string.IsNullOrEmpty((string)fieldData.Value) ? null : fieldData.Value.ToString();
                                    break;
                            }
                        }
                        fields.Add(field);
                    }
                    list.ListFields = fields;

                    lists.Add(list);
                }
                project.ProjectLists = lists;
                return project;
            }
            catch
            {
                return null;
            }
        }

        private List<ItemEntity> LoadProjectListData(int projectId, int listId, string camlQueryString)
        {
            try
            {
                ProjectEntity project = null;
                if (5 != projectId)
                    project = LoadPanelProjectData(projectId);
                else
                    project = Project5;
                if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return null;
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var listFields = listData.ListFields.Select(x => x.FieldTitle).ToList();
                    var context =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                        };
                    var announcementsList = context.Web.Lists.GetByTitle(listData.ListTitle);
                    var camlQuery = new CamlQuery();//
                    if (string.IsNullOrEmpty(camlQueryString))
                        camlQueryString = @"<View Scope='RecursiveAll'><Query>
                                            <OrderBy><FieldRef Name='ID' Ascending='True'/></OrderBy>   
                                            </Query><RowLimit>500</RowLimit></View>";
                    camlQuery.ViewXml = camlQueryString;
                    //var collListItem = announcementsList.GetItems(camlQuery);
                    //context.Load(collListItem);
                    //context.ExecuteQuery();
                    //
                    List<ListItem> items = new List<ListItem>();
                    do
                    {
                        ListItemCollection listItemCollection = announcementsList.GetItems(camlQuery);
                        context.Load(listItemCollection);
                        context.ExecuteQuery();

                        //Adding the current set of ListItems in our single buffer
                        items.AddRange(listItemCollection);
                        //Reset the current pagination info
                        camlQuery.ListItemCollectionPosition = listItemCollection.ListItemCollectionPosition;

                    } while (camlQuery.ListItemCollectionPosition != null);
                    //
                    var result = new List<ItemEntity>();
                    foreach (Microsoft.SharePoint.Client.ListItem oListItem in items/*collListItem*/)
                    {
                        var filedsData = new List<KeyValueEntity>();
                        var idKey = 0;
                        foreach (KeyValuePair<string, object> data in oListItem.FieldValues)
                        {
                            var key = data.Key;
                            if (key == "ID")
                                idKey = int.Parse(data.Value.ToString());
                            else if (listFields.Contains(data.Key))
                                try
                                {
                                    string dataValue = "";
                                    if (null != data.Value && "Microsoft.SharePoint.Client.FieldLookupValue" == data.Value.ToString())
                                    {
                                        var childIdField = oListItem[data.Key.ToString()] as FieldLookupValue;
                                        dataValue = childIdField.LookupValue;
                                    }
                                    else
                                        dataValue = data.Value != null ? data.Value.ToString() : "";
                                    filedsData.Add(new KeyValueEntity
                                    {
                                        Key = data.Key,
                                        Value = dataValue
                                    });
                                }
                                catch (Exception e)
                                {
                                    //ignore
                                }

                        }
                        result.Add(new ItemEntity { ItemId = idKey, ItemContents = filedsData });
                    }

                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;//JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
            }
        }
        
        private void BulkProjectListData(int listId, int bulkSize)
        {

        }

        public FieldContentEntry GetRefahiProjectListData(int listId, int listContentId)
        {
            var project = LoadPanelProjectData(3);
            if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return null;
            try
            {
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var listFields = listData.ListFields.Select(x => x.FieldTitle).ToList();
                    var context =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                        };
                    var announcementsList = context.Web.Lists.GetByTitle(listData.ListTitle);
                    var oListItem = announcementsList.GetItemById(listContentId);
                    context.Load(oListItem);
                    context.ExecuteQuery();

                    var content = new FieldContentEntry();
                    var filedsData = new List<KeyValueEntity>();//Dictionary<string, string>();
                    if (oListItem == null) return null;

                    foreach (var data in oListItem.FieldValues)
                    {
                        try
                        {
                            if (listFields.Contains(data.Key))
                                filedsData.Add(new KeyValueEntity { Key = data.Key, Value = data.Value.ToString() });//data.Key, data.Value.ToString());
                        }
                        catch (Exception e)
                        {
                            //ignore
                        }
                    }
                    content.DataId = listContentId;
                    content.Contents = filedsData;

                    //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد
                    //var attachFiles = GetListItemAttachFiles(listData.ProjectUrl, listData.ListTitle, listContentId);
                    ////var keys = LoadListItemAttachFileKeys(listData.ProjectUrl, attachFiles);
                    //var attachFileContents = LoadListItemAttachFileContents(listData.ProjectUrl, attachFiles);
                    //content.AttachFiles = attachFileContents;
                    //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد

                    /*
                    FileCollection files = GetAttachments(context, announcementsList, oListItem);
                    foreach (var file in files)
                    {
                        FileInformation fileInfo = File.OpenBinaryDirect(context, file.ServerRelativeUrl);
                        context.ExecuteQuery();
                        using (var memoryStream = new System.IO.MemoryStream())
                        {
                            fileInfo.Stream.CopyTo(memoryStream);

                            content.AttachFiles.Add(new AttachFileEntry
                            {
                                FileName = file.Name,
                                FileContent = memoryStream.ToArray()
                            });
                        };
                    }
                    */

                    return content; //JsonConvert.SerializeObject(content);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null; //JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
            }
        }

        //public int ListItemCount(int listId)
        //{
        //    var project = LoadRefahiProjectData();
        //    if (null == project) return -1;
        //    if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return -1;
        //    var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
        //    if (null == listData) return -1;
        //    try
        //    {
        //        var context =
        //        new ClientContext(project.ProjectUrl)
        //        {
        //            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
        //        };
        //        var projectList = context.Web.Lists.GetByTitle(listData.ListTitle);
        //        ListItemCollection itemCollection = projectList.GetItems(CamlQuery.CreateAllItemsQuery());
        //        context.Load(itemCollection);
        //        context.ExecuteQuery();
        //        return itemCollection.Count;
        //    }
        //    catch (Exception ex)
        //    {
        //        return -1;
        //    }
        //}

        private string nationalCode2personnelCode(string nationalCode)
        {
            string personnelCodeQuery = @"<View><Query><Where><Eq>                                
                                            <FieldRef Name='NationalNo'/>
                                            <Value Type='Text'>" + nationalCode + @"</Value>
                                            </Eq></Where><RowLimit>1</RowLimit></Query></View>";
            var listItem = LoadProjectListData(10, 53, personnelCodeQuery);
            if (null == listItem && 1 != listItem.Count) return "";
            return listItem[0].ItemContents.SingleOrDefault(kvp => kvp.Key == "PersonelCode").Value;
        }

        public List<FieldContentEntry> LoadListFiles(int projectId, int listId, List<int> listContentsId)
        {
            var project = LoadPanelProjectData(projectId);
            if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return null;

            try
            {
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var listFields = listData.ListFields.Select(x => x.FieldTitle).ToList();

                    ICredentials credentials =
                        new System.Net.NetworkCredential(@"qom\rafieim", "drived");
                    var context =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = credentials
                        };
                    var announcementsList = context.Web.Lists.GetByTitle(listData.ListTitle);
                    var oListItems = announcementsList.GetItems(CamlQuery.CreateAllItemsQuery());
                    context.Load(oListItems);
                    context.ExecuteQuery();

                    var result = new List<FieldContentEntry>();
                    foreach (var oListItem in oListItems)
                    {
                        if (oListItem == null) continue;
                        if (!listContentsId.Contains(oListItem.Id)) continue;
                        //
                        //AttachmentCollection oAttachments = oListItem.AttachmentFiles;
                        //context.Load(oAttachments);
                        //context.ExecuteQuery();

                        //foreach (Attachment oAttachment in oAttachments)
                        //{
                        //    Console.WriteLine("File Name - " + oAttachment.FileName);
                        //}
                        //
                        var content = new FieldContentEntry();
                        var filedsData = new List<KeyValueEntity>();// Dictionary<string, string>();
                        foreach (var data in oListItem.FieldValues)
                        {
                            if (data.Key == "FileLeafRef")
                            {
                                var fileContent = DownloadFileViaRestApiBytes(project.ProjectUrl, "AttachmentFile", data.Value.ToString());
                                if (fileContent == null) continue;
                                string base64String = Convert.ToBase64String(fileContent, 0, fileContent.Length);
                                filedsData.Add(new KeyValueEntity { Key = data.Key, Value = data.Value.ToString() }); //(data.Key, base64String);
                            }
                            else if (listFields.Contains(data.Key))
                                try
                                {
                                    filedsData.Add(new KeyValueEntity { Key = data.Key, Value = data.Value.ToString() }); //(data.Key, data.Value.ToString());
                                }
                                catch
                                {
                                    //ignore
                                }

                        }
                        content.DataId = oListItem.Id;
                        content.Contents = filedsData;
                        result.Add(content);
                    }
                    return result;//JsonConvert.SerializeObject(result);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;//JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
            }
        }

        //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد
        //private FileCollection GetAttachments(ClientContext context, List list, ListItem item)
        //{
        //    context.Load(list, l => l.RootFolder.ServerRelativeUrl);
        //    context.Load(context.Site, s => s.Url);
        //    context.ExecuteQuery();

        //    Folder attFolder = context.Web.GetFolderByServerRelativeUrl(list.RootFolder.ServerRelativeUrl + "/Attachments/" + item.Id);
        //    FileCollection files = attFolder.Files;

        //    context.Load(files, fs => fs.Include(f => f.ServerRelativeUrl, f => f.Name, f => f.ServerRelativeUrl));
        //    context.ExecuteQuery();

        //    return files;
        //}
        //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد

        /*private FileCollection GetAttachments(ClientContext context, List list, ListItem item)
        {
            context.Load(list, l => l.RootFolder.ServerRelativeUrl);
            context.Load(context.Site, s => s.Url);
            context.ExecuteQuery();

            Folder attFolder = context.Web.GetFolderByServerRelativeUrl(list.RootFolder.ServerRelativeUrl + "/Attachments/" + item.Id);
            FileCollection files = attFolder.Files;

            context.Load(files, fs => fs.Include(f => f.ServerRelativeUrl, f => f.Name, f => f.ServerRelativeUrl));
            context.ExecuteQuery();

            return files;
        }*/

        public List<ResultEntity> SaveProjectListData(int projectId, int listId,
            ListFieldsContentEntry listItem)//, bool deleteAttachFiles, List<AttachFileEntry> attachFiles)
        {
            var project = LoadPanelProjectData(projectId);
            if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return null;

            try
            {
                var exceptions = new List<ExceptionEntity>();
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var listFields = listData.ListFields.Select(x => x.FieldTitle).ToList();

                    var clientContext =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                        };
                    clientContext.RequestTimeout = 1000000;
                    var oWebsite = clientContext.Web;
                    var oList = oWebsite.Lists.GetByTitle(listData.ListTitle);
                    var affctedList = new List<ResultEntity>();


                    var contentId = listItem.Id;
                    if (contentId != 0)
                    {
                        try
                        {
                            var newItem = oList.GetItemById(listItem.Id);
                            foreach (var item in listItem.Contents)
                            {
                                var key = item.ContentKey;
                                if (listFields.Contains(key))
                                    newItem[key] = string.IsNullOrEmpty(item.ContentValue) ? null : item.ContentValue;
                            }

                            newItem.Update();
                            clientContext.ExecuteQuery();

                            /*
                            //غیر فعال سازی فایل
                            try
                            {
                                var attachList = new List<string>();
                                var fileFolders = oWebsite.Folders;
                                foreach (var library in fileFolders)
                                {
                                    if (library.Name != "DocumnentLibraryFiles") continue;
                                    if (deleteAttachFiles)
                                    {
                                        var olAttachmentFiles = "".Split(',').ToList();
                                        var oldFiles = library.Files;
                                        foreach (var oldFile in oldFiles)
                                        {
                                            if (!olAttachmentFiles.Contains(oldFile.Name)) continue;
                                            oldFile.DeleteObject();
                                            library.Update();
                                        }
                                    }

                                    foreach (var attachFile in attachFiles)
                                    {
                                        var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
                                        library.Files.Add(new FileCreationInformation
                                        {
                                            Content = attachFile.FileContent,
                                            Overwrite = true,
                                            Url = url
                                        });
                                        library.Update();
                                        attachList.Add(url);
                                    }
                                }
                                if (attachList.Count > 0)
                                {
                                    newItem = oList.GetItemById(listItem.Id);
                                    newItem["AttachmentFiles"] = string.Join(",", attachList);
                                    newItem.Update();
                                    clientContext.ExecuteQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                            }
                            */

                            /*
                            try
                            {
                                if (attachFiles.Count != 0)
                                {
                                    foreach (var attachFile in attachFiles)
                                    {
                                        var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
                                        string attachmentPath = $"/Lists/{listData.ListTitle}/Attachments/{contentId}/{url}";
                                        System.IO.Stream stream = new System.IO.MemoryStream(attachFile.FileContent);
                                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, attachmentPath, stream, true);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                            }
                            */

                            affctedList.Add(
                                new ResultEntity { DataId = listItem.Id, ResultStatus = "SuccessUpdate" });
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(new ExceptionEntity { DataId = contentId, Exception = ex.Message });
                        }
                    }
                    else
                    {
                        try
                        {
                            var itemCreateInfo = new ListItemCreationInformation();
                            var newItem = oList.AddItem(itemCreateInfo);
                            foreach (var item in listItem.Contents)
                            {
                                var key = item.ContentKey;
                                if (listFields.Contains(key))
                                    newItem[key] = string.IsNullOrEmpty(item.ContentValue) ? null : item.ContentValue;
                            }

                            newItem.Update();
                            clientContext.ExecuteQuery();

                            /*
                            //غیر فعال سازی فایل
                            try
                            {
                                var attachList = new List<string>();
                                var fileFolders = oWebsite.Folders;
                                foreach (var library in fileFolders)
                                {
                                    if (library.Name != "DocumnentLibraryFiles") continue;
                                    

                                    foreach (var attachFile in attachFiles)
                                    {
                                        var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
                                        library.Files.Add(new FileCreationInformation
                                        {
                                            Content = attachFile.FileContent,
                                            Overwrite = true,
                                            Url = url
                                        });
                                        library.Update();
                                        attachList.Add(url);
                                    }
                                }
                                if (attachList.Count > 0)
                                {
                                    newItem = oList.GetItemById(listItem.Id);
                                    newItem["AttachmentFiles"] = string.Join(",", attachList);
                                    newItem.Update();
                                    clientContext.ExecuteQuery();
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                            }
                            */

                            /*
                            try
                            {
                                if (attachFiles.Count != 0)
                                {
                                    foreach (var attachFile in attachFiles)
                                    {
                                        var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
                                        string attachmentPath = $"/Lists/{listData.ListTitle}/Attachments/{contentId}/{url}";
                                        System.IO.Stream stream = new System.IO.MemoryStream(attachFile.FileContent);
                                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, attachmentPath, stream, true);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                            }
                            */

                            affctedList.Add(new ResultEntity { DataId = newItem.Id, ResultStatus = "Success Insert" });
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                        }
                    }


                    //var result = JsonConvert.SerializeObject(affctedList);
                    if (exceptions.Count != 0)
                    //result += JsonConvert.SerializeObject(exceptions);
                    {
                        var exceptionsResult = new List<ResultEntity>();
                        foreach (var ex in exceptions)
                        {
                            exceptionsResult.Add(new ResultEntity
                            {
                                DataId = -1,
                                ResultStatus = $"Faild Insert {ex}"
                            });
                        }
                        affctedList.AddRange(exceptionsResult);
                    }
                    //return result;

                    return affctedList;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null; //JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
            }
        }

        public List<ResultEntity> DeleteProjectListData(int projectId, int listId,
            List<int> listContentIds)
        {
            var project = LoadPanelProjectData(projectId);
            if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return null;

            try
            {
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var exceptions = new List<ExceptionEntity>();
                    var clientContext =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                        };
                    var oWebsite = clientContext.Web;
                    var oList = oWebsite.Lists.GetByTitle(listData.ListTitle);

                    var affctedList = new List<ResultEntity>();
                    foreach (var id in listContentIds)
                    {
                        try
                        {
                            var newItem = oList.GetItemById(id);
                            newItem.DeleteObject();
                            clientContext.ExecuteQuery();
                            affctedList.Add(new ResultEntity { DataId = id, ResultStatus = "Success Delete" });
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(new ExceptionEntity { DataId = id, Exception = ex.Message });
                        }
                    }

                    /*
                    //غیر فعال سازی فایل
                    try
                    {
                        var fileFolders = oWebsite.Folders;
                        foreach (var library in fileFolders)
                        {
                            if (library.Name != "DocumnentLibraryFiles") continue;
                            var olAttachmentFiles = "".Split(',').ToList();
                            var oldFiles = library.Files;
                            foreach (var oldFile in oldFiles)
                            {
                                if (!olAttachmentFiles.Contains(oldFile.Name)) continue;
                                oldFile.DeleteObject();
                                library.Update();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new ExceptionEntity { Exception = ex.Message });
                    }
                    */

                    //var result = JsonConvert.SerializeObject(affctedList);
                    if (exceptions.Count != 0)
                    {
                        //result += JsonConvert.SerializeObject(exceptions);
                        var exceptionsResult = new List<ResultEntity>();
                        foreach (var ex in exceptions)
                        {
                            exceptionsResult.Add(new ResultEntity
                            {
                                DataId = -1,
                                ResultStatus = $"Faild Delete {ex}"
                            });
                        }
                        affctedList.AddRange(exceptionsResult);
                    }
                    return affctedList;
                }
                return null;
            }
            catch (Exception ex)
            {
                return
                    null; //;JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
            }
        }

        //public int Count(int projectId, int listId, string filterFieldName, string filterFieldValue)
        //{
        //    try
        //    {
        //        return LoadProjectListData(projectId, listId, filterFieldName, filterFieldValue).Count;
        //    }
        //    catch (Exception ex)
        //    {
        //        return -1;
        //    }
        //}

        //public string Max(int projectId, int listId, string fieldInternalName, string filterFieldName, string filterFieldValue)
        //{
        //    try
        //    {
        //        var listItem = ProjectListData(projectId, listId, filterFieldName, filterFieldValue, false, "False", fieldInternalName, "1");
        //        if (1 == listItem.Count)
        //            return listItem[0].ItemContents.SingleOrDefault(x => x.Key == fieldInternalName).Value.ToString();
        //        else
        //            return "Error in Computing!";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Exception : " + ex.Message;
        //    }
        //}

        //public string Min(int projectId, int listId, string fieldInternalName, string filterFieldName, string filterFieldValue)
        //{
        //    try
        //    {
        //        var listItem = ProjectListData(projectId, listId, filterFieldName, filterFieldValue, false, "True", fieldInternalName, "1");
        //        if (1 == listItem.Count)
        //            return listItem[0].ItemContents.SingleOrDefault(x => x.Key == fieldInternalName).Value.ToString();
        //        else
        //            return "Error in Computing!";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Exception : " + ex.Message;
        //    }
        //}

        //public string Sum(int projectId, int listId, string fieldInternalName, string filterFieldName, string filterFieldValue)
        //{
        //    string result = "";
        //    //try
        //    //{
        //    //    var itemEntityList = LoadProjectListData(projectId, listId, filterFieldName, filterFieldValue);
        //    //    int sum = 0;
        //    //    foreach (var itemEntity in itemEntityList)
        //    //        sum += int.Parse(itemEntity.ItemContents.SingleOrDefault(x => x.Key == fieldInternalName).Value.ToString());
        //    //    result = sum.ToString();
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return "Exception : " + ex.Message;
        //    //}
        //    return result;
        //}

        //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد
        //private List<string> GetListItemAttachFiles(string url, string listTitle, int listContentId)
        //{
        //    var attachFiles = new List<string>();
        //    try
        //    {
        //        var context =
        //            new ClientContext(url)
        //            {
        //                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
        //            };
        //        var announcementsList = context.Web.Lists.GetByTitle(listTitle);
        //        var camlQuery = new CamlQuery
        //        {
        //            ViewXml =
        //                "<Where><Eq><FieldRef Name='ID' /><Value Type='Integer'>" + listContentId +
        //                "</Value></Eq></Where>"
        //        };
        //        var oListItem = announcementsList.GetItems(camlQuery).SingleOrDefault();
        //        context.Load(oListItem);
        //        context.ExecuteQuery();

        //        foreach (var data in oListItem.FieldValues)
        //        {
        //            if (data.Key == "AttachmentFiles")
        //                attachFiles.AddRange(data.Value.ToString().Split(',').ToList());
        //        }
        //    }
        //    catch
        //    {
        //        //ignore
        //    }

        //    return attachFiles;
        //}

        //private List<int> LoadListItemAttachFileKeys(string url, List<string> attachFileNames)
        //{
        //    var keys = new List<int>();
        //    try
        //    {
        //        var context =
        //            new ClientContext(url)
        //            {
        //                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
        //            };
        //        var announcementsList = context.Web.Lists.GetByTitle("AttachmentFile");
        //        var camlQuery = new CamlQuery
        //        {
        //            ViewXml =
        //                "<View><RowLimit>20000</RowLimit></View><OrderBy><FieldRef Name='ID' Ascending='False'/></Now></OrderBy>"
        //        };
        //        var oListItems = announcementsList.GetItems(camlQuery);
        //        context.Load(oListItems);
        //        context.ExecuteQuery();
        //        foreach (var listItem in oListItems)
        //        {
        //            foreach (var fieldValue in listItem.FieldValues)
        //            {
        //                if(fieldValue.Key == "Title")
        //                    if (attachFileNames.Contains(fieldValue.Value))
        //                    {
        //                        keys.Add(listItem.Id);
        //                    }
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        //ignore
        //    }

        //    return keys.Distinct().ToList();
        //}

        //private List<AttachFileEntry> LoadListItemAttachFileContents(string url, List<string> attachFileNames)
        //{
        //    //http://eportal.qom.ir/edesk/parvanesakhtemani/AttachmentFile/2019-06-18T15-12-57_rafieim-%D8%B5%D9%81%D8%AD%D9%87%20%D8%A7%D9%88%D9%84%20%D8%B4%D9%86%D8%A7%D8%B3%D9%86%D8%A7%D9%85%D9%87--FormResource.png
        //    var attachFiles = new List<AttachFileEntry>();
        //    ICredentials credentials = new NetworkCredential("rafieim", "drived", "qom");
        //    foreach (var fileName in attachFileNames)
        //    {
        //        var fileContent = DownloadFileViaRestApiBytes(url, credentials, "AttachmentFile", fileName);
        //        if(fileContent == null) continue;
        //        attachFiles.Add(new AttachFileEntry
        //        {
        //            FileName = fileName,
        //            FileContent = fileContent
        //        });
        //    }
        //    return attachFiles;

        //    /*try
        //    {
        //        Microsoft.SharePoint.SPSite sps = new SPSite(url);
        //        SPWeb spwCurrent = sps.OpenWeb();
        //        SPList splDocumentLibrary = spwCurrent.Lists["AttachmentFiles"];
        //        SPListItem spliDocument = splDocumentLibrary.GetItemById(5 -- Your specific document item --);
        //        SPFile spfDocument = spliDocument.File;
        //        byte[] binFile = spfDocument.OpenBinary();


        //        var context =
        //            new ClientContext(url)
        //            {
        //                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
        //            };
        //        var announcementsList = context.Web.Lists.GetByTitle("AttachmentFiles");
        //        var oListItems = announcementsList.GetItems(CamlQuery.CreateAllItemsQuery(1000));
        //        context.Load(oListItems);
        //        context.ExecuteQuery();

        //        foreach (var listItem in oListItems)
        //        {
        //            var file = listItem.File;
        //                FileInformation fileInfo = WebRequestMethods.File.OpenBinaryDirect(context, file.ServerRelativeUrl);
        //                context.ExecuteQuery();
        //                using (var memoryStream = new System.IO.MemoryStream())
        //                {
        //                    fileInfo.Stream.CopyTo(memoryStream);


        //                };
        //            }



        //        var attachFile = oListItem;
        //        attachFile.op
        //    }
        //    catch
        //    {
        //        //ignore
        //    }*/
        //}
        //این قطعه کد برای خواندن فایل بود که به متد مجزایی در سرویس تبدیل شد
        private byte[] DownloadFileViaRestApiBytes(string webUrl, string documentLibName,
            string fileName)
        {
            try
            {
                webUrl = webUrl.EndsWith("/") ? webUrl.Substring(0, webUrl.Length - 1) : webUrl;
                string webRelativeUrl = null;
                if (webUrl.Split('/').Length > 3)
                {
                    webRelativeUrl = "/" + webUrl.Split(new char[] { '/' }, 4)[3];
                }
                else
                {
                    webRelativeUrl = "";
                }

                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                    client.Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived");
                    Uri endpointUri = new Uri(webUrl + "/_api/web/GetFileByServerRelativeUrl('" + webRelativeUrl + "/" +
                                              documentLibName + "/" + fileName + "')/$value");

                    //string result = client.DownloadString(endpointUri);
                    byte[] data = client.DownloadData(endpointUri);
                    return data;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //#region File
        //public List<string> SaveFiles(int projectId, List<AttachFileEntry> attachFiles)
        //{
        //    if (attachFiles.Count == 0) return null;
        //    var urlList = new List<string>();
        //    try
        //    {
        //        var project = LoadPanelProjectData(projectId);
        //        var exceptions = new List<ExceptionEntity>();
        //        var clientContext =
        //            new ClientContext(
        //                project.ProjectUrl)
        //            {
        //                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
        //            };
        //        clientContext.RequestTimeout = 1000000;
        //        var oWebsite = clientContext.Web;
        //        var fileFolders = oWebsite.Folders;
        //        foreach (var library in fileFolders)
        //        {
        //            if (library.Name != "AttachmentFile") continue;
        //            foreach (var attachFile in attachFiles)
        //            {
        //                var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
        //                library.Files.Add(new FileCreationInformation
        //                {
        //                    Content = attachFile.FileContent,
        //                    Overwrite = true,
        //                    Url = url
        //                });
        //                library.Update();
        //                urlList.Add(url);
        //            }
        //            //foreach (var attachFile in attachFiles)
        //            //{
        //            //    var url = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(attachFile.FileName);
        //            //    string attachmentPath = $"/Lists/{listData.ListTitle}/Attachments/{contentId}/{url}";
        //            //    System.IO.Stream stream = new System.IO.MemoryStream(attachFile.FileContent);
        //            //    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, attachmentPath, stream, true);
        //            //}
        //        }
        //        return urlList;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null; //JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
        //    }
        //}

        //public List<StreamFileEntry> ReadFiles(int projectId, List<string> urlList)
        //{
        //    try
        //    {
        //        var project = LoadPanelProjectData(projectId);
        //        ICredentials credentials =
        //                    new System.Net.NetworkCredential(@"qom\rafieim", "drived");
        //        var context =
        //            new ClientContext(
        //                project.ProjectUrl)
        //            {
        //                Credentials = credentials
        //            };
        //        var announcementsList = context.Web.Lists.GetByTitle("DocumnentLibrary");
        //        var oListItems = announcementsList.GetItems(CamlQuery.CreateAllItemsQuery());
        //        context.Load(oListItems);
        //        context.ExecuteQuery();
        //        var streamFiles = new List<StreamFileEntry>();
        //        foreach (var item in oListItems)
        //        {
        //            if (urlList.Contains(item["FileLeafRef"]))
        //            {
        //                var stream = Microsoft.SharePoint.Client.File.OpenBinaryDirect(context, (string)item["FileRef"]).Stream;
        //                streamFiles.Add(new StreamFileEntry
        //                {
        //                    FileName = item["FileLeafRef"].ToString(),
        //                    StreamContent = stream
        //                });
        //            }
        //        }
        //        return streamFiles;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;//JsonConvert.SerializeObject(new ExceptionEntity { DataId = -1, Exception = ex.Message });
        //    }
        //}
        //#endregion

        private FieldUserValue GetUserInformaitonListData(string userName)
        {
            try
            {
                var tempContext =
                            new ClientContext(
                                Project5.ProjectUrl)
                            {
                                Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                            };
                User user = tempContext.Web.EnsureUser(userName);
                if (null != user)
                {
                    tempContext.Load(user);
                    tempContext.ExecuteQuery();
                    if (null != user)
                    {
                        int id = user.Id;
                        FieldUserValue userFieldValue = new FieldUserValue();
                        userFieldValue.LookupId = user.Id;
                        return userFieldValue;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception in GetUserInformationListData" + ex.Message);
                return null;
            }
        }

        private RecallEntityStructure GeneralRecallsInformation(string camlQuery)
        {
            var activeRecallList = LoadProjectListData(5, 41, camlQuery).ToList().Where(x => x.ItemContents.Any(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06" && ic.Value == "فعال")).ToList();
            var recallDetails = new List<RecallEntity>();
            foreach (var recallItem in activeRecallList)
            {
                var newRecall = new RecallEntity();
                newRecall.RecallID = recallItem.ItemId;
                newRecall.RecallTitle = null != recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "Title") ?
                    recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "Title").Value : "";
                //
                newRecall.RecallNumber = null != recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0634__x0645__x0627__x0631__x06") ?
                    recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0634__x0645__x0627__x0631__x06").Value.ToString() : "";
                newRecall.RecallStatus = null != recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06") ?
                    recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06").Value.ToString() : "";
                newRecall.SecretariatDemander = null != recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x062f__x0628__x06cc__x0631__x06") ?
                    recallItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x062f__x0628__x06cc__x0631__x06").Value.ToString() : "";
                recallDetails.Add(newRecall);
            }
            return new RecallEntityStructure() { ListCount = recallDetails.Count.ToString(), RecallInformationList = recallDetails };
        }
        
        public RecallEntityStructure RecallsInformation()
        {
            return GeneralRecallsInformation("");
        }

        private SuggestionFieldEntityStructure GeneralSuggestionFieldInformation(string camlQuery)
        {
            var activeSuggestionFieldList = LoadProjectListData(5, 42, camlQuery).ToList().Where(x => x.ItemContents.Any(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06" && ic.Value == "فعال")).ToList();
            var suggestionFieldList = new List<SuggestionFieldEntity>();
            foreach (var suggestionFieldItem in activeSuggestionFieldList)
            {
                var newSuggestionField = new SuggestionFieldEntity();
                newSuggestionField.SuggestionFieldID = suggestionFieldItem.ItemId;
                newSuggestionField.SuggestionFieldTitle = null != suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "Title") ?
                    suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "Title").Value : "";
                newSuggestionField.SuggestionFieldNumber = null != suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0634__x0645__x0627__x0631__x06") ?
                    suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0634__x0645__x0627__x0631__x06").Value : "";
                newSuggestionField.SuggestionFieldStatus = null != suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06") ?
                    suggestionFieldItem.ItemContents.SingleOrDefault(ic => ic.Key == "_x0648__x0636__x0639__x06cc__x06").Value : "";
                suggestionFieldList.Add(newSuggestionField);
            }
            return new SuggestionFieldEntityStructure() { ListCount = suggestionFieldList.Count.ToString(), SuggestionFieldList = suggestionFieldList };
        }

        public SuggestionFieldEntityStructure SuggestionFieldInformation()
        {
            return GeneralSuggestionFieldInformation("");
        }

        private PersonnelPostEntity PersonnelPost(string nationalCode)
        {
            string PersonnelPostQuery = @"<View><Query><Where><Eq>                                
                                            <FieldRef Name='NationalCode'/>
                                            <Value Type='Text'>" + nationalCode + @"</Value>
                                            </Eq></Where><RowLimit>1</RowLimit></Query></View>";
            var listItem = LoadProjectListData(10, 69, PersonnelPostQuery);
            if (null == listItem || 1 != listItem.Count) return null;
            return new PersonnelPostEntity()
            {
                NationalCode = nationalCode,
                PersonnelCode = listItem[0].ItemContents.SingleOrDefault(kve => kve.Key == "PersonelCode").Value.ToString(),
                BusinessLocationChart = listItem[0].ItemContents.SingleOrDefault(kve => kve.Key == "BusinessLocationChart4").Value.ToString()
            };
        }

        private PersonnelStatusEntity PersonnelStatus(string nationalCode)
        {
            string PersonnelStatusQuery = @"<View><Query><Where><Eq>                                
                                            <FieldRef Name='NationalCode'/>
                                            <Value Type='Text'>" + nationalCode + @"</Value>
                                            </Eq></Where><RowLimit>1</RowLimit></Query></View>";
            var listItem = LoadProjectListData(10, 71, PersonnelStatusQuery);
            if (null == listItem || 1 != listItem.Count) return null;
            return new PersonnelStatusEntity()
            {
                NationalCode = nationalCode,
                PersonnelCode = listItem[0].ItemContents.SingleOrDefault(kve => kve.Key == "PersonelCode").Value.ToString(),
                RecruitmentType = listItem[0].ItemContents.SingleOrDefault(kve => kve.Key == "RecruitmentType").Value.ToString()
            };
        }

        public SuggestionInsertionStatus SubmitCitizenSuggestion(string SuggestionTitle, string SuggestionField, string Recall, 
            string MainSuggesterPhone, string mainSuggesterNationalCode, string mainSuggesterCellPhone,
            string mainSuggesterUserName, string mainSuggesterName, string mainSuggesterFamily,
            string SuggestionImplementationEffect,
            string MoneySavedFirstYearImplementation,
            string SuggestionImplementedBefore,
            string CurrentSituationProblemsShortComings,
            string ProsAndCons,
            string FacilityEquipmentManpower,
            string FullDescription,
            string urgency,
            string urgencyReason)
        {
            CitizenSuggestionEntity citizenSuggestion = new CitizenSuggestionEntity();
            citizenSuggestion.SuggestionTitle = SuggestionTitle;
            citizenSuggestion.SuggestionField = SuggestionField;
            citizenSuggestion.Recall = Recall;
            citizenSuggestion.MainSuggesterPhone = MainSuggesterPhone;
            citizenSuggestion.mainSuggesterNationalCode = mainSuggesterNationalCode;
            citizenSuggestion.mainSuggesterCellPhone = mainSuggesterCellPhone;
            citizenSuggestion.mainSuggesterUserName = mainSuggesterUserName;
            citizenSuggestion.mainSuggesterName = mainSuggesterName;
            citizenSuggestion.mainSuggesterFamily = mainSuggesterFamily;
            citizenSuggestion.implementationEffect = SuggestionImplementationEffect;
            citizenSuggestion.moneySavedFirstYearImplementation = MoneySavedFirstYearImplementation;
            citizenSuggestion.implementedBefore = SuggestionImplementedBefore;
            citizenSuggestion.currentSituationProblemsShortComings = CurrentSituationProblemsShortComings;
            citizenSuggestion.prosAndCons = ProsAndCons;
            citizenSuggestion.facilityEquipmentManpower = FacilityEquipmentManpower;
            citizenSuggestion.fullDescription = FullDescription;
            citizenSuggestion.urgency = urgency;
            citizenSuggestion.urgencyReason = urgencyReason;
            //
            var suggestionItem = SubmitSuggestion(citizenSuggestion);
            UpdateCommonProperties(suggestionItem, Citizen, Individual, "100", "1",
                GetUserInformaitonListData("i:0#.f|fbamembershipprovider|" + citizenSuggestion.mainSuggesterUserName));
            try
            {
                suggestionItem.Update();
                myContext.ExecuteQuery();
            }
            catch (Exception ex)
            {
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful inserting a citizen suggestion 
                                                " + ex.Message;
                return returnValue;
            }
            //
            SuggestionInsertionStatus result = new SuggestionInsertionStatus();
            result.Title = SuggestionTitle;
            result.InsertionStatus = "Successfully inserted a citizen suggestion";
            return result;
        }

        public SuggestionInsertionStatus SubmitSomeCitizensSuggestion(string SuggestionTitle, string SuggestionField, string Recall,
            string MainSuggesterPhone, string mainSuggesterNationalCode, string mainSuggesterCellPhone,
            string mainSuggesterUserName, string mainSuggesterName, string mainSuggesterFamily,
            string mainParticipationPercentage, string SuggestionDistributorCount,
            string SuggestionImplementationEffect,
            string MoneySavedFirstYearImplementation,
            string SuggestionImplementedBefore,
            string CurrentSituationProblemsShortComings,
            string ProsAndCons,
            string FacilityEquipmentManpower,
            string FullDescription,
            string urgency,
            string urgencyReason,
            string SubSuggester_1CellPhone, string SubSuggester_2CellPhone, string SubSuggester_3CellPhone, string SubSuggester_4CellPhone, 
            string SubSuggester_1UserName, string SubSuggester_2UserName, string SubSuggester_3UserName, string SubSuggester_4UserName,
            string Sub_1ParticipationPercentage, string Sub_2ParticipationPercentage, string Sub_3ParticipationPercentage, 
            string Sub_4ParticipationPercentage)
        {
            var someCitizensSuggestion = new SomeCitizenSuggestionEntity();
            someCitizensSuggestion.SuggestionTitle = SuggestionTitle;
            someCitizensSuggestion.SuggestionField = SuggestionField;
            someCitizensSuggestion.Recall = Recall;
            someCitizensSuggestion.MainSuggesterPhone = MainSuggesterPhone;
            someCitizensSuggestion.mainSuggesterNationalCode = mainSuggesterNationalCode;
            someCitizensSuggestion.mainSuggesterCellPhone = mainSuggesterCellPhone;
            someCitizensSuggestion.mainSuggesterUserName = mainSuggesterUserName;
            someCitizensSuggestion.mainSuggesterName = mainSuggesterName;
            someCitizensSuggestion.mainSuggesterFamily = mainSuggesterFamily;
            someCitizensSuggestion.implementationEffect = SuggestionImplementationEffect;
            someCitizensSuggestion.moneySavedFirstYearImplementation = MoneySavedFirstYearImplementation;
            someCitizensSuggestion.implementedBefore = SuggestionImplementedBefore;
            someCitizensSuggestion.currentSituationProblemsShortComings = CurrentSituationProblemsShortComings;
            someCitizensSuggestion.prosAndCons = ProsAndCons;
            someCitizensSuggestion.facilityEquipmentManpower = FacilityEquipmentManpower;
            someCitizensSuggestion.fullDescription = FullDescription;
            someCitizensSuggestion.urgency = urgency;
            someCitizensSuggestion.urgencyReason = urgencyReason;
            //
            try
            {
                var suggestionItem = SubmitSuggestion(someCitizensSuggestion);
                UpdateCommonProperties(suggestionItem, Citizen, Group, mainParticipationPercentage,
                    SuggestionDistributorCount,
                    GetUserInformaitonListData("i:0#.f|fbamembershipprovider|" + someCitizensSuggestion.mainSuggesterUserName));
                //
                UpdateCellPhone(suggestionItem, SubSuggester_1CellPhone, SubSuggester_2CellPhone,
                    SubSuggester_3CellPhone, SubSuggester_4CellPhone);
                //
                UpdateUserName(suggestionItem, SubSuggester_1UserName, SubSuggester_2UserName, SubSuggester_3UserName,
                    SubSuggester_4UserName, "i:0#.f|fbamembershipprovider|");
                //
                UpdateParticipationPercentage(suggestionItem, Sub_1ParticipationPercentage,
                    Sub_2ParticipationPercentage, Sub_3ParticipationPercentage, Sub_4ParticipationPercentage);
                suggestionItem.Update();
                myContext.ExecuteQuery();
                SuggestionInsertionStatus result = new SuggestionInsertionStatus();
                result.Title = SuggestionTitle;
                result.InsertionStatus = "Successfully inserted some citizen suggestion";
                return result;
            }
            catch (Exception ex)
            {
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful inserting some citizen suggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        private void UpdateCommonEmployeeProperties(ListItem suggestionItem, string mainSuggesterNationalCode)
        {
            var pse = PersonnelStatus(mainSuggesterNationalCode);
            suggestionItem["_x0646__x0648__x0639__x0627__x06"] = null != pse ? pse.RecruitmentType : null;
            //
            var ppe = PersonnelPost(mainSuggesterNationalCode);
            suggestionItem["_x0645__x062d__x0644__x062e__x06"] = null != ppe ? ppe.BusinessLocationChart : null;
        }

        public SuggestionInsertionStatus SubmitEmployeeSuggestion(string SuggestionTitle, string SuggestionField, string Recall,
            string MainSuggesterPhone, string mainSuggesterNationalCode, string mainSuggesterCellPhone,
            string mainSuggesterUserName, string mainSuggesterName, string mainSuggesterFamily,
            string SuggestionImplementationEffect,
            string MoneySavedFirstYearImplementation,
            string SuggestionImplementedBefore,
            string CurrentSituationProblemsShortComings,
            string ProsAndCons,
            string FacilityEquipmentManpower,
            string FullDescription,
            string urgency,
            string urgencyReason)
        {
            var employeeSuggestion = new EmployeeSuggetionEntity();
            employeeSuggestion.SuggestionTitle = SuggestionTitle;
            employeeSuggestion.SuggestionField = SuggestionField;
            employeeSuggestion.Recall = Recall;
            employeeSuggestion.MainSuggesterPhone = MainSuggesterPhone;
            employeeSuggestion.mainSuggesterNationalCode = mainSuggesterNationalCode;
            employeeSuggestion.mainSuggesterCellPhone = mainSuggesterCellPhone;
            employeeSuggestion.mainSuggesterUserName = mainSuggesterUserName;
            employeeSuggestion.mainSuggesterName = mainSuggesterName;
            employeeSuggestion.mainSuggesterFamily = mainSuggesterFamily;
            employeeSuggestion.implementationEffect = SuggestionImplementationEffect;
            employeeSuggestion.moneySavedFirstYearImplementation = MoneySavedFirstYearImplementation;
            employeeSuggestion.implementedBefore = SuggestionImplementedBefore;
            employeeSuggestion.currentSituationProblemsShortComings = CurrentSituationProblemsShortComings;
            employeeSuggestion.prosAndCons = ProsAndCons;
            employeeSuggestion.facilityEquipmentManpower = FacilityEquipmentManpower;
            employeeSuggestion.fullDescription = FullDescription;
            employeeSuggestion.urgency = urgency;
            employeeSuggestion.urgencyReason = urgencyReason;
            //
            try
            {
                var suggestionItem = SubmitSuggestion(employeeSuggestion);
                UpdateCommonProperties(suggestionItem, Employee, Individual, "100", "1",
                    GetUserInformaitonListData("Qom\\" + employeeSuggestion.mainSuggesterUserName));
                UpdateCommonEmployeeProperties(suggestionItem, employeeSuggestion.mainSuggesterNationalCode);
                suggestionItem.Update();
                myContext.Load(suggestionItem);
                myContext.ExecuteQuery();
                SuggestionInsertionStatus result = new SuggestionInsertionStatus();
                result.Title = SuggestionTitle;
                result.InsertionStatus = "Successfully inserted an employee suggestion";
                return result;
                //int id = suggestionItem.Id;
            }
            catch (Exception ex)
            {
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful inserting an employee suggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        public SuggestionInsertionStatus SubmitSomeEmployeesSuggestion(string SuggestionTitle, string SuggestionField, string Recall,
            string MainSuggesterPhone, string mainSuggesterNationalCode, string mainSuggesterCellPhone,
            string mainSuggesterUserName, string mainSuggesterName, string mainSuggesterFamily, 
            string mainParticipationPercentage,
            string SuggestionDistributorCount,
            string SuggestionImplementationEffect,
            string MoneySavedFirstYearImplementation,
            string SuggestionImplementedBefore,
            string CurrentSituationProblemsShortComings,
            string ProsAndCons,
            string FacilityEquipmentManpower,
            string FullDescription,
            string urgency,
            string urgencyReason,
            string SubSuggester_1CellPhone,
            string SubSuggester_2CellPhone,
            string SubSuggester_3CellPhone,
            string SubSuggester_4CellPhone,
            string SubSuggester_1NationalCode,
            string SubSuggester_2NationalCode,
            string SubSuggester_3NationalCode,
            string SubSuggester_4NationalCode,
            string SubSuggester_1UserName,
            string SubSuggester_2UserName,
            string SubSuggester_3UserName,
            string SubSuggester_4UserName,
            string Sub_1ParticipationPercentage,
            string Sub_2ParticipationPercentage,
            string Sub_3ParticipationPercentage,
            string Sub_4ParticipationPercentage)
        {
            var someEmployeesSuggestion = new SomeEmployeeSuggetionEntity();
            someEmployeesSuggestion.SuggestionTitle = SuggestionTitle;
            someEmployeesSuggestion.SuggestionField = SuggestionField;
            someEmployeesSuggestion.Recall = Recall;
            someEmployeesSuggestion.MainSuggesterPhone = MainSuggesterPhone;
            someEmployeesSuggestion.mainSuggesterNationalCode = mainSuggesterNationalCode;
            someEmployeesSuggestion.mainSuggesterCellPhone = mainSuggesterCellPhone;
            someEmployeesSuggestion.mainSuggesterUserName = mainSuggesterUserName;
            someEmployeesSuggestion.mainSuggesterName = mainSuggesterName;
            someEmployeesSuggestion.mainSuggesterFamily = mainSuggesterFamily;
            someEmployeesSuggestion.implementationEffect = SuggestionImplementationEffect;
            someEmployeesSuggestion.moneySavedFirstYearImplementation = MoneySavedFirstYearImplementation;
            someEmployeesSuggestion.implementedBefore = SuggestionImplementedBefore;
            someEmployeesSuggestion.currentSituationProblemsShortComings = CurrentSituationProblemsShortComings;
            someEmployeesSuggestion.prosAndCons = ProsAndCons;
            someEmployeesSuggestion.facilityEquipmentManpower = FacilityEquipmentManpower;
            someEmployeesSuggestion.fullDescription = FullDescription;
            someEmployeesSuggestion.urgency = urgency;
            someEmployeesSuggestion.urgencyReason = urgencyReason;
            //
            try
            {
                var suggestionItem = SubmitSuggestion(someEmployeesSuggestion);
                UpdateCommonProperties(suggestionItem, Employee, Group, mainParticipationPercentage,
                    SuggestionDistributorCount,
                    GetUserInformaitonListData("Qom\\" + someEmployeesSuggestion.mainSuggesterUserName));
                UpdateCommonEmployeeProperties(suggestionItem, someEmployeesSuggestion.mainSuggesterNationalCode);
                //
                UpdateCellPhone(suggestionItem, SubSuggester_1CellPhone, SubSuggester_2CellPhone, SubSuggester_3CellPhone,
                    SubSuggester_4CellPhone);
                //
                var someBusinessLocation1 = PersonnelPost(SubSuggester_1NationalCode);
                var stringBusinessLocation1 = null != someBusinessLocation1 ? someBusinessLocation1.BusinessLocationChart : "";
                //
                var someBusinessLocation2 = PersonnelPost(SubSuggester_2NationalCode);
                var stringBusinessLocation2 = null != someBusinessLocation2 ? someBusinessLocation2.BusinessLocationChart : "";
                //
                var someBusinessLocation3 = PersonnelPost(SubSuggester_3NationalCode);
                var stringBusinessLocation3 = null != someBusinessLocation3 ? someBusinessLocation3.BusinessLocationChart : "";
                //
                var someBusinessLocation4 = PersonnelPost(SubSuggester_4NationalCode);
                var stringBusinessLocation4 = null != someBusinessLocation4 ? someBusinessLocation4.BusinessLocationChart : "";
                //
                UpdateEmployeesBusinessLocation(suggestionItem,
                    stringBusinessLocation1,
                    stringBusinessLocation2,
                    stringBusinessLocation3,
                    stringBusinessLocation4);
                //
                var RecruitmentType1 = PersonnelStatus(SubSuggester_1NationalCode);
                var stringRecruitmentType1 = null != RecruitmentType1 ? RecruitmentType1.RecruitmentType : "";
                //
                var RecruitmentType2 = PersonnelStatus(SubSuggester_2NationalCode);
                var stringRecruitmentType2 = null != RecruitmentType2 ? RecruitmentType2.RecruitmentType : "";
                //
                var RecruitmentType3 = PersonnelStatus(SubSuggester_3NationalCode);
                var stringRecruitmentType3 = null != RecruitmentType3 ? RecruitmentType3.RecruitmentType : "";
                //
                var RecruitmentType4 = PersonnelStatus(SubSuggester_4NationalCode);
                var stringRecruitmentType4 = null != RecruitmentType4 ? RecruitmentType4.RecruitmentType : "";
                //
                UpdateEmployeesRecruitmentType(suggestionItem,
                    stringRecruitmentType1,
                    stringRecruitmentType2,
                    stringRecruitmentType3,
                    stringRecruitmentType4);
                //
                UpdateUserName(suggestionItem, SubSuggester_1UserName, SubSuggester_2UserName, SubSuggester_3UserName,
                    SubSuggester_4UserName, "Qom\\");
                //
                UpdateParticipationPercentage(suggestionItem, Sub_1ParticipationPercentage, Sub_2ParticipationPercentage,
                     Sub_3ParticipationPercentage, Sub_4ParticipationPercentage);
                suggestionItem.Update();
                myContext.ExecuteQuery();
                SuggestionInsertionStatus result = new SuggestionInsertionStatus();
                result.Title = SuggestionTitle;
                result.InsertionStatus = "Successfully inserted some employee suggestion";
                return result;
            }
            catch(Exception ex)
            {
                SuggestionInsertionStatus returnValue = new SuggestionInsertionStatus();
                returnValue.Title = SuggestionTitle;
                returnValue.InsertionStatus = @"Unsuccessful inserting some employee suggestion
                                                " + ex.Message;
                return returnValue;
            }
        }

        private void UpdateCommonProperties(ListItem suggestionItem, string suggesterType, string suggestionType,
            string mainParticipationPercentage, string contributorsCount, FieldUserValue mainUserName)
        {
            suggestionItem["_x0646__x0648__x0639__x0020__x06"] = suggestionType;
            suggestionItem["_x0646__x0648__x0639__x067e__x06"] = suggesterType;
            suggestionItem["_x062f__x0631__x0635__x062f__x06"] = mainParticipationPercentage;
            suggestionItem["_x062a__x0639__x062f__x0627__x06"] = contributorsCount;
            suggestionItem["_x067e__x06cc__x0634__x0646__x060"] = mainUserName;
        }

        private void UpdateEmployeesBusinessLocation(ListItem suggestionItem, string BusinessLocation1
            , string BusinessLocation2
            , string BusinessLocation3
            , string BusinessLocation4)
        {
            suggestionItem["_x0645__x062d__x0644__x062e__x060"] = !string.IsNullOrEmpty(BusinessLocation1) ? BusinessLocation1 : "";
            suggestionItem["_x0645__x062d__x0644__x062e__x061"] = !string.IsNullOrEmpty(BusinessLocation2) ? BusinessLocation2 : "";
            suggestionItem["_x0645__x062d__x0644__x062e__x062"] = !string.IsNullOrEmpty(BusinessLocation3) ? BusinessLocation3 : "";
            suggestionItem["_x0645__x062d__x0644__x062e__x063"] = !string.IsNullOrEmpty(BusinessLocation4) ? BusinessLocation4 : "";
        }

        private void UpdateEmployeesRecruitmentType(ListItem suggestionItem
            ,string EmployeesRecruitmentType1
            , string EmployeesRecruitmentType2
            , string EmployeesRecruitmentType3
            , string EmployeesRecruitmentType4)
        {
            suggestionItem["_x0646__x0648__x0639__x0627__x060"] = !string.IsNullOrEmpty(EmployeesRecruitmentType1) ? EmployeesRecruitmentType1 : "";
            suggestionItem["_x0646__x0648__x0639__x0627__x061"] = !string.IsNullOrEmpty(EmployeesRecruitmentType2) ? EmployeesRecruitmentType2 : "";
            suggestionItem["_x0646__x0648__x0639__x0627__x062"] = !string.IsNullOrEmpty(EmployeesRecruitmentType3) ? EmployeesRecruitmentType3 : "";
            suggestionItem["_x0646__x0648__x0639__x0627__x063"] = !string.IsNullOrEmpty(EmployeesRecruitmentType4) ? EmployeesRecruitmentType4 : "";
        }

        private void UpdateCellPhone(ListItem suggestionItem
            , string cellPhone1
            , string cellPhone2
            , string cellPhone3
            , string cellPhone4)
        {
            suggestionItem["_x062a__x0644__x0641__x0646__x064"] = !string.IsNullOrEmpty(cellPhone1) ? cellPhone1 : "";
            suggestionItem["_x062a__x0644__x0641__x0646__x060"] = !string.IsNullOrEmpty(cellPhone2) ? cellPhone2 : "";
            suggestionItem["_x062a__x0644__x0641__x0646__x062"] = !string.IsNullOrEmpty(cellPhone3) ? cellPhone3 : "";
            suggestionItem["_x062a__x0644__x0641__x0646__x063"] = !string.IsNullOrEmpty(cellPhone4) ? cellPhone4 : "";
        }
        
        private void UpdateUserName(ListItem suggestionItem
            , string userName1
            , string userName2
            , string userName3
            , string userName4
            , string extra)
        {
            suggestionItem["_x067e__x06cc__x0634__x0646__x061"] = GetUserInformaitonListData(extra + userName1);
            suggestionItem["_x067e__x06cc__x0634__x0646__x062"] = GetUserInformaitonListData(extra + userName2);
            suggestionItem["_x067e__x06cc__x0634__x0646__x063"] = GetUserInformaitonListData(extra + userName3);
            suggestionItem["_x067e__x06cc__x0634__x0646__x064"] = GetUserInformaitonListData(extra + userName4);
        }

        private void UpdateParticipationPercentage(ListItem suggestionItem
            , string ParticipationPercentage1
            , string ParticipationPercentage2
            , string ParticipationPercentage3
            , string ParticipationPercentage4)
        {
            suggestionItem["_x062f__x0631__x0635__x062f__x060"] = !string.IsNullOrEmpty(ParticipationPercentage1) ? ParticipationPercentage1 : "";
            suggestionItem["_x062f__x0631__x0635__x062f__x062"] = !string.IsNullOrEmpty(ParticipationPercentage2) ? ParticipationPercentage2 : "";
            suggestionItem["_x062f__x0631__x0635__x062f__x061"] = !string.IsNullOrEmpty(ParticipationPercentage3) ? ParticipationPercentage3 : "";
            suggestionItem["_x062f__x0631__x0635__x062f__x063"] = !string.IsNullOrEmpty(ParticipationPercentage4) ? ParticipationPercentage4 : "";
        }

        private ListItem SubmitSuggestion(SuggestionInformation si)
        {
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem suggestionItem = MyList.AddItem(itemCreateInfo);
            try
            {
                suggestionItem["Title"] = !string.IsNullOrEmpty(si.SuggestionTitle) ? si.SuggestionTitle : "";
                //
                if (!string.IsNullOrEmpty(si.SuggestionField))
                {
                    string suggestionFieldQuery = @"<View><Query><Where><Eq>                                
                                            <FieldRef Name='Title'/>
                                            <Value Type='Text'>" + si.SuggestionField + @"</Value>
                                            </Eq></Where><RowLimit>1</RowLimit></Query></View>";
                    var suggestionField = GeneralSuggestionFieldInformation(suggestionFieldQuery).SuggestionFieldList;
                    if (1 == suggestionField.Count && null != suggestionField[0])
                    {
                        var lookupField = new FieldLookupValue();
                        lookupField.LookupId = suggestionField[0].SuggestionFieldID;
                        suggestionItem["_x0632__x0645__x06cc__x0646__x060"] = lookupField;
                    }
                    else
                        suggestionItem["_x0632__x0645__x06cc__x0646__x060"] = null;
                }
                else
                    suggestionItem["_x0632__x0645__x06cc__x0646__x060"] = null;
                //
                if (!string.IsNullOrEmpty(si.Recall))
                {
                    string recallQuery = @"<View><Query><Where><Eq>                                
                                            <FieldRef Name='Title'/>
                                            <Value Type='Text'>" + si.Recall + @"</Value>
                                            </Eq></Where><RowLimit>1</RowLimit></Query></View>";
                    var recallField = GeneralRecallsInformation(recallQuery).RecallInformationList;
                    if (1 == recallField.Count && null != recallField[0])
                    {
                        var lookupField = new FieldLookupValue();
                        lookupField.LookupId = recallField[0].RecallID;
                        suggestionItem["_x0641__x0631__x0627__x062e__x06"] = lookupField;
                    }
                    else
                        suggestionItem["_x0641__x0631__x0627__x062e__x06"] = null;
                }
                else
                    suggestionItem["_x0641__x0631__x0627__x062e__x06"] = null;
                //
                suggestionItem["_x062a__x0644__x0641__x0646__x00"] = !string.IsNullOrEmpty(si.mainSuggesterCellPhone) 
                    ? si.mainSuggesterCellPhone : "";
                suggestionItem["_x062a__x0627__x062b__x06cc__x06"] = !string.IsNullOrEmpty(si.implementationEffect)
                    ? si.implementationEffect : "";
                suggestionItem["_x0635__x0631__x0641__x0647__x00"] = !string.IsNullOrEmpty(si.moneySavedFirstYearImplementation)
                    ? si.moneySavedFirstYearImplementation : "";

                suggestionItem["_x0622__x06cc__x0627__x0020__x06"] = !string.IsNullOrEmpty(si.implementedBefore)
                    ? si.implementedBefore : "";
                suggestionItem["_x0634__x0631__x062d__x0020__x06"] = !string.IsNullOrEmpty(si.currentSituationProblemsShortComings)
                    ? si.currentSituationProblemsShortComings : "";

                suggestionItem["_x0645__x0632__x0627__x06cc__x06"] = !string.IsNullOrEmpty(si.prosAndCons)
                    ? si.prosAndCons : "";
                suggestionItem["_x0627__x0645__x06a9__x0627__x06"] = !string.IsNullOrEmpty(si.facilityEquipmentManpower)
                    ? si.facilityEquipmentManpower : "";

                suggestionItem["_x062a__x0648__x0636__x06cc__x06"] = !string.IsNullOrEmpty(si.fullDescription)
                    ? si.fullDescription : "";
                suggestionItem["_x0641__x0648__x0631__x06cc__x06"] = !string.IsNullOrEmpty(si.urgency)
                    ? si.urgency : "";
                suggestionItem["_x062f__x0644__x06cc__x0644_"] = !string.IsNullOrEmpty(si.urgencyReason)
                    ? si.urgencyReason : "";
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception Inserting in Suggestion List : " + ex.Message);
            }
            return suggestionItem;
        }

        public SuggestionImpactStructure SuggestionImpacts()
        {
            var suggestionImpactList = new List<SuggestionImpactEntity>()
            {new SuggestionImpactEntity() {SuggestionImpactID = 1, SuggestionImpactTitle = "مادی" },
            new SuggestionImpactEntity() {SuggestionImpactID = 2, SuggestionImpactTitle = "غیر مادی" } };
            return new SuggestionImpactStructure() { SuggestionImpactsCount = suggestionImpactList.Count, SuggestionImpacts = suggestionImpactList };
        }

        public SuggestionPrioritySturcture SuggestionPriorities()
        {
            var suggestionPriority = new List<SuggestionPriorityEntity>()
            {
                new SuggestionPriorityEntity() {SuggestionPriorityID = 1, SuggestionPriorityTitle = "عادی" },
                new SuggestionPriorityEntity() {SuggestionPriorityID = 2, SuggestionPriorityTitle = "بالا" },
                new SuggestionPriorityEntity() {SuggestionPriorityID = 3, SuggestionPriorityTitle = "فوری" }
            };
            return new SuggestionPrioritySturcture() { SuggestionPriorityCount = suggestionPriority.Count, SuggestionPriorities = suggestionPriority };
        }

        public SearchResultStructure SearchSuggestionTitle(string title)
        {
            var context =
                        new ClientContext(
                            Project5.ProjectUrl)
                        {
                            Credentials = new System.Net.NetworkCredential(@"qom\rafieim", "drived")
                        };

            var theList = context.Web.Lists.GetByTitle(Project5.ProjectLists.SingleOrDefault(pl => pl.ListId == 40).ListTitle);
            string titleQuery = @"<View><Query><Where><Contains>                                
                                            <FieldRef Name='Title'/>
                                            <Value Type='Text'>" + title + @"</Value>
                                            </Contains></Where></Query></View>";
            var camlQuery = new CamlQuery();
            camlQuery.ViewXml = titleQuery;
            var collListItem = theList.GetItems(camlQuery);
            context.Load(collListItem, ListItem => ListItem.Include(i => i["Title"]));
            context.ExecuteQuery();
            var list = collListItem.ToList().Select(listItem => listItem["Title"].ToString()).ToList();
            return new SearchResultStructure() { ResultCount = list.Count, SearchResults = list };
        }

        public void GetItemAttachments(int projectId, int listId, List<int> listContentsId)
        {
            var project = LoadPanelProjectData(projectId);
            if (!project.ProjectLists.Select(x => x.ListId).Contains(listId)) return;

            try
            {
                var listData = project.ProjectLists.SingleOrDefault(x => x.ListId == listId);
                if (listData != null)
                {
                    var listFields = listData.ListFields.Select(x => x.FieldTitle).ToList();

                    ICredentials credentials =
                        new System.Net.NetworkCredential(@"qom\rafieim", "drived");
                    var context =
                        new ClientContext(
                            project.ProjectUrl)
                        {
                            Credentials = credentials
                        };
                    var announcementsList = context.Web.Lists.GetByTitle(listData.ListTitle);
                    var oListItems = announcementsList.GetItems(CamlQuery.CreateAllItemsQuery());
                    context.Load(oListItems);
                    context.ExecuteQuery();

                    var result = new List<FieldContentEntry>();
                    foreach (var oListItem in oListItems)
                    {
                        if (oListItem == null) continue;
                        if (!listContentsId.Contains(oListItem.Id)) continue;
                        context.Load(context.Site, Site => Site.Url);
                        context.ExecuteQuery();//+ listData.ListTitle
                        string temp = (context.Site.Url + "/Lists/List" + "/Attachments/" + oListItem["ID"]);
                        Folder folder = context.Web.GetFolderByServerRelativeUrl(temp);
                        context.Load(folder);
                        context.ExecuteQuery();
                        context.Load(folder.Files);
                        context.ExecuteQuery();
                        foreach (Microsoft.SharePoint.Client.File oFile in folder.Files)
                        {
                            FileInfo myFileinfo = new FileInfo(oFile.Name);
                            var stream = new FileInfo("").Open(FileMode.Open);
                            Microsoft.SharePoint.Client.File.SaveBinaryDirect(context, "Attachment", stream, true);
                            WebClient client1 = new WebClient();
                            client1.Credentials = credentials;
                            byte[] fileContents =
                                  client1.DownloadData("http://siteaction.net" +
                                  oFile.ServerRelativeUrl);
                            FileStream fStream = new FileStream(@"C:\temp" +
                                  oFile.Name, FileMode.Create);
                            fStream.Write(fileContents, 0, fileContents.Length);
                            fStream.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }
    }
}