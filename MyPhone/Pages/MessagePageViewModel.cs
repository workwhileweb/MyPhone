﻿using CommunityToolkit.Mvvm.ComponentModel;
using GoodTimeStudio.MyPhone.Models;
using GoodTimeStudio.MyPhone.OBEX;
using MixERP.Net.VCards.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoodTimeStudio.MyPhone.Pages
{
    public class MessagePageViewModel : ObservableObject
    {

        public ObservableCollection<Conversation> Conversations { get; set; }

        public MessagePageViewModel()
        {
            Conversations = new ObservableCollection<Conversation>();

            var vcard = new MixERP.Net.VCards.VCard
            {
                FormattedName = "John",
                Telephones = new List<Telephone> { new() { Number = "13805121234" } }
            };
            Conversations.Add(new Conversation
            {
                ContactInfo = vcard,
                Messages = new ObservableCollection<BMessage> { new(BMessageStatus.READ, "telecom", "telecom", vcard, "UTF-8", 0, "Test message") },
            });

            var vcard2 = new MixERP.Net.VCards.VCard
            {
                FormattedName = "Haha",
                Telephones = new List<Telephone> { new() { Number = "13805121234" } }
            };
            Conversations.Add(new Conversation
            {
                ContactInfo = vcard2,
                Messages = new ObservableCollection<BMessage> { new(BMessageStatus.READ, "telecom", "telecom", vcard, "UTF-8", 0, "Hello world") },
            });
        }
    }
}
