namespace TsmRptLibrary
{
    internal interface ICem
    {
    }

    internal interface INonCem
    {
    }

    internal interface ITimed : INonCem, ICem
    {
    }

    internal interface ITelemed : INonCem
    {
    }

    internal interface IMinMax : INonCem
    {
    }
}