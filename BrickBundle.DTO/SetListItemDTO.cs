namespace BrickBundle.DTO
{
    public class SetListItemDTO
    {
        public long ID { get; set; }
        public string SetNum { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public long ThemeID { get; set; }
        public string ThemeName { get; set; }
        public int NumParts { get; set; }
        public float Percentage { get; set; }
    }
}
