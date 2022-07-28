﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage.Streams;

namespace MyPhone.OBEX.Map
{
    public class MasClient : ObexClient
    {

        //bb582b40-420c-11db-b0de-0800200c9a66
        public static readonly byte[] MAS_UUID = new byte[] { 0xBB, 0x58, 0x2B, 0x40, 0x42, 0x0C, 0x11, 0xDB, 0xB0, 0xDE, 0x08, 0x00, 0x20, 0x0C, 0x9A, 0x66 };
        
        /// <remarks>
        /// Not null after connected.
        /// </remarks>
        private Int32ValueHeader? _connectionIdHeader; 

        public MasClient(IInputStream inputStream, IOutputStream outputStream) : base(inputStream, outputStream)
        {
        }

        protected override void OnConnected(ObexPacket connectionResponse)
        {
            if (connectionResponse.Headers.TryGetValue(HeaderId.ConnectionId, out IObexHeader connectionId))
            {
                _connectionIdHeader = (Int32ValueHeader?)connectionId;
            }
            else
            {
                throw new ObexRequestException(connectionResponse.Opcode, "The MasServer does not provide a ConnectionId, abort.");
            }
        }

        public async Task SetFolderAsync(SetPathMode mode, string folderName = "")
        {
            ObexPacket packet = new MapSetPathRequestPacket(mode, folderName);
            packet.Headers[HeaderId.ConnectionId] = _connectionIdHeader!;
            await RunObexRequestAsync(packet);
        }

        /// <summary>
        /// Retrieve messages listing from MSE
        /// </summary>
        /// <param name="maxListCount">Maximum number of messages listed. Must be GREATER THAN 0.</param>
        /// <param name="folderName">The name of the folder.</param>
        /// <remarks>If you try to get the listing size, please call <see cref="GetMessageListingSizeAsync"/> instead.</remarks>
        /// <returns>message handle list</returns>
        /// TODO: return Messages-Listing objects
        public async Task<List<string>> GetMessagesListingAsync(ushort maxListCount, string folderName = "telecom")
        {
            ObexPacket packet = new ObexPacket(
                new ObexOpcode(ObexOperation.Get, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/MAP-msg-listing"),
                new UnicodeStringValueHeader(HeaderId.Name, folderName),
                new AppParamHeader(new AppParameter(AppParamTagId.MaxListCount, maxListCount))
                );

            Console.WriteLine($"Sending GetMessageListing request ");
            ObexPacket resp = await RunObexRequestAsync(packet);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(((BodyHeader)resp.Headers[HeaderId.Body]).Value);
            XmlNodeList list = xml.SelectNodes("/MAP-msg-listing/msg/@handle");
            List<string> ret = new List<string>();
            Console.WriteLine("Message handle list: ");
            foreach (XmlNode n in list)
            {
                if (n.Value != null)
                {
                    Console.WriteLine(n.Value);
                    ret.Add(n.Value);
                }
            }

            return ret;
        }

        public async Task<int> GetMessageListingSizeAsync(string folderName = "telecom")
        {
            ObexPacket packet = new ObexPacket(
                new ObexOpcode(ObexOperation.Get, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/MAP-msg-listing"),
                new UnicodeStringValueHeader(HeaderId.Name, folderName),
                new AppParamHeader(new AppParameter(AppParamTagId.MaxListCount, 0))
                );

            if (packet.Headers.TryGetValue(HeaderId.ApplicationParameters, out IObexHeader header)) 
            {
                AppParamHeader appParamHeader = (AppParamHeader)header;
                AppParameter p = appParamHeader.AppParameters.Where(param => param.TagId == AppParamTagId.ListingSize).FirstOrDefault();
            }

            return 0;
        }

        public async Task<BMessage> GetMessageAsync(string messageHandle)
        {
            ObexPacket packet = new ObexPacket(
                new ObexOpcode(ObexOperation.Get, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/message"),
                new UnicodeStringValueHeader(HeaderId.Name, messageHandle),
                new AppParamHeader(
                    new AppParameter(AppParamTagId.Attachment, MasConstants.ATTACHMENT_ON),
                    new AppParameter(AppParamTagId.Charset, MasConstants.CHARSET_UTF8)
                    )
                );

            Console.WriteLine("Sending GetMessage request ");

            ObexPacket resp = await RunObexRequestAsync(packet);

            // "EndOfBody" has been copied to "Body" by ObexClient
            string bMsgStr = ((BodyHeader)resp.Headers[HeaderId.Body]).Value!;

            BMessage bMsg;
            try
            {
                BMessageNode bMsgNode = BMessageNode.Parse(bMsgStr);
                bMsg = new BMessage(
                    status: bMsgNode.Attributes["STATUS"] == "UNREAD" ? MessageStatus.UNREAD : MessageStatus.READ,
                    type: bMsgNode.Attributes["TYPE"],
                    folder: bMsgNode.Attributes["FOLDER"],
                    charset: bMsgNode.ChildrenNode["BENV"].ChildrenNode["BBODY"].Attributes["CHARSET"],
                    length: int.Parse(bMsgNode.ChildrenNode["BENV"].ChildrenNode["BBODY"].Attributes["LENGTH"]),
                    body: bMsgNode.ChildrenNode["BENV"].ChildrenNode["BBODY"].ChildrenNode["MSG"].Value!,
                    sender: MixERP.Net.VCards.Deserializer.GetVCard(bMsgNode.ChildrenNode["VCARD"].ToString())
                    );
            }
            catch (BMessageException ex)
            {
                throw new ObexException($"Failed to get message (handle: {messageHandle}) from MSE. The MSE send back a invalid response", ex);
            }

            return bMsg;
        }

        public async Task SetNotificationRegistrationAsync(bool enableNotification)
        {
            byte flag = (byte)(enableNotification ? 1 : 0);

            ObexPacket packet = new ObexPacket(
                new ObexOpcode(ObexOperation.Put, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/MAP-NotificationRegistration"),
                new AppParamHeader(new AppParameter(AppParamTagId.NotificationStatus, flag)),
                new BytesHeader(HeaderId.EndOfBody, 0x30)
                );

            Console.WriteLine("Sending RemoteNotificationRegister request");
            await RunObexRequestAsync(packet);
        }

        public async Task GetMasInstanceInformationAsync()
        {
            ObexPacket packet = new ObexPacket(
                new (ObexOperation.Get, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/MASInstanceInformation"),
                new AppParamHeader(new AppParameter(AppParamTagId.MASInstanceID, MAS_UUID))
                );

            Console.WriteLine($"Sending GetMASInstanceInformation request ");
            await RunObexRequestAsync(packet);
        }

        /// <summary>
        /// Get the list of children folder name in the current folder
        /// </summary>
        /// <param name="maxListCount">The maximum number of folders to retrieve (default 1024).</param>
        /// <param name="listStartOffset">The offset of the first entry of the returned folder</param>
        /// <returns>List of children folder name</returns>
        public async Task<List<string>> GetFolderListingAsync(ushort? maxListCount = null, ushort? listStartOffset = null)
        {
            ObexPacket packet = new ObexPacket(
                new (ObexOperation.Get, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-obex/folder-listing")
            );
            if (maxListCount != null || listStartOffset != null)
            {
                AppParamHeader appParamHeader = new();
                if (maxListCount != null)
                {
                    appParamHeader.AppParameters.AddLast(new AppParameter(AppParamTagId.MaxListCount, maxListCount.Value));
                }
                if (listStartOffset != null)
                {
                    appParamHeader.AppParameters.AddLast(new AppParameter(AppParamTagId.ListStartOffset, listStartOffset.Value));
                }
            }

            Console.WriteLine("sending GetFolderList request");

            ObexPacket resp = await RunObexRequestAsync(packet);

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(((BodyHeader)resp.Headers[HeaderId.EndOfBody]).Value);
            XmlNodeList list = xml.SelectNodes("/folder-listing/folder/@name");
            List<string> ret = new List<string>();
            Console.WriteLine("Folder list: ");
            foreach (XmlNode n in list)
            {
                if (n.Value != null)
                {
                    Console.WriteLine(n.Value);
                    ret.Add(n.Value);
                }
            }

            return ret;
        }

        public async Task PushMessage()
        {
            ObexPacket packet = new ObexPacket(
                new ObexOpcode(ObexOperation.Put, true),
                _connectionIdHeader!,
                new AsciiStringValueHeader(HeaderId.Type, "x-bt/message"),
                new AsciiStringValueHeader(HeaderId.Name, "telecom/msg/inbox"),
                new AppParamHeader(new AppParameter(AppParamTagId.Charset, MasConstants.CHARSET_UTF8)),
                new AsciiStringValueHeader(HeaderId.EndOfBody, "test pushing message from MCE")
                );

            Console.WriteLine("sending PushMessage request ");

            await RunObexRequestAsync(packet);
        }

    }

    public static class MasClientExtensions
    {
        /// <summary>
        /// Get all children folders' name of current folder
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetChildrenFoldersAsync(this MasClient client)
        {
            List<string> ret = new();

            bool lastPage = false;
            ushort offset = 0;
            const int default_size = 1024;
            while (!lastPage)
            {
                var foldersName = await client.GetFolderListingAsync(listStartOffset: offset);
                if (foldersName.Count < default_size)
                {
                    lastPage = true;
                }

                ret.AddRange(foldersName);
            }

            return ret;
        }

        /// <summary>
        /// Traverese the entire folder tree.
        /// </summary>
        /// <returns>Root folder and children folder</returns>
        public static async Task<SmsFolder> TraverseFolderAsync(this MasClient client)
        {
            await client.SetFolderAsync(SetPathMode.BackToRoot);

            SmsFolder root = new SmsFolder("Root");
            Stack<SmsFolder> folders = new Stack<SmsFolder>();
            folders.Push(root);
            SmsFolder pre = root;

            while (folders.Count > 0)
            {
                SmsFolder current = folders.Pop();

                if (current != root && current.Parent != pre.Parent)
                {
                    if (current.Parent == pre)
                    {
                        await client.SetFolderAsync(SetPathMode.EnterFolder, current.Name);
                    }
                    else if (pre.Parent != null && pre.Parent.Parent == current.Parent)
                    {
                        await client.SetFolderAsync(SetPathMode.BackToParent);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unreachable code reached!");
                    }
                }

                List<string> subFoldersName = await client.GetChildrenFoldersAsync();
                subFoldersName.ForEach(f =>
                {
                    //await client.GetMessageListing(0, f);
                    
                    SmsFolder smsFolder = new SmsFolder(f, current);

                    current.Children.Add(smsFolder);
                    folders.Push(smsFolder);
                });

                pre = current;
            }

            return root;
        }
        
    }

}
