namespace BrickBundle.DTO
{
    public class ListPartsCategoriesColorsDTO
    {
        public PartListItemDTO[] Parts { get; set; }
        public ColorListItemDTO[] Colors { get; set; }
        public string[] Categories { get; set; }
    }
}
