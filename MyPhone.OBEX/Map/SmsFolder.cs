using System.Collections.Generic;

namespace GoodTimeStudio.MyPhone.OBEX.Map
{
    public class SmsFolder
    {
        public string Name { get; }
        /// <summary>
        /// Number of accessible messages in this folder
        /// </summary>
        public int MessageCount { get; }
        public IList<SmsFolder> Children { get; }
        public IList<string> MessageHandles { get; }

        public SmsFolder? Parent { get; }

        public SmsFolder(string folderName, int messageCount, SmsFolder? parent = null)
        {
            Name = folderName;
            MessageCount = messageCount;
            Children = new List<SmsFolder>();
            MessageHandles = new List<string>();
            Parent = parent;
        }

        public SmsFolder(string folderName, SmsFolder? parent = null) : this(folderName, 0, parent) { }
    }
}
