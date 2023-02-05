namespace workapi.Models
{
    public record SellInfo(string nums, string[] imgs, string[] Prices, string[] Names, string[] Spec);
    public record Goldinfo(string TitleUrl, string[] Carousels, SellInfo Sells);
}
