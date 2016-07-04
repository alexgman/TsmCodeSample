namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal class MonkeySpaceTypeDto
    {
        public short IsAutomatic { get; set; } = default(short);

        public short IsForWalkDesign { get; set; } = default(int);

        public short IsUserRequestable { get; set; } = default(short);

        public string RequestDescription { get; set; } = string.Empty;

        public int RequestType { get; set; } = default(int);
    }
}