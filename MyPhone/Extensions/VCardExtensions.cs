using GoodTimeStudio.MyPhone.Data;
using MixERP.Net.VCards;

namespace GoodTimeStudio.MyPhone.Extensions
{
    public static class VCardExtensions
    {
        public static Contact ToContact(this VCard card)
        {
            return new Contact
            {
                MiddleName = card.MiddleName ?? string.Empty,
                NickName = card.NickName ?? string.Empty,
                FirstName = card.FirstName ?? string.Empty,
                LastName = card.LastName ?? string.Empty,
                FormattedName = card.FormattedName ?? string.Empty,
                Organization = card.Organization ?? string.Empty,
                OrganizationalUnit = card.OrganizationalUnit ?? string.Empty,
                Detail = card
            };
        }
    }
}
