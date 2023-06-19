namespace GoodTimeStudio.MyPhone.Data
{
    public interface IIdentifiable<TKey>
    {
        TKey Id { get; set; }
    }
}
