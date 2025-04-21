namespace eBookStore.Web.ViewModels
{
    public class TopSellingBookVM: BookVM
    {
        public int CopiesSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
