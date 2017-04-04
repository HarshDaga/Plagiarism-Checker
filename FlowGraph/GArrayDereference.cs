using System.Text.RegularExpressions;

#pragma warning disable CS1591

public class GArrayDereference : GVar
{
	public static readonly string pattern = @"^MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]$";
	public static readonly string pattern2 = @"^MEM\[(\(.*\))?(?<name>[\w\.]+)\]$";

	public string Offset { get; set; }

	public GArrayDereference ( string str )
	{
		if ( Regex.IsMatch ( str, pattern ) )
		{
			var match = Regex.Match ( str, pattern );
			Name = match.Groups["name"].Value;
			Offset = match.Groups["offset"].Value;
		}
		else if ( Regex.IsMatch ( str, pattern2 ) )
		{
			var match = Regex.Match ( str, pattern2 );
			Name = match.Groups["name"].Value;
			Offset = "0";
		}
	}

	public static bool Matches ( string str ) => Regex.IsMatch ( str, pattern ) || Regex.IsMatch ( str, pattern2 );

	public override int GetHashCode ( ) => Name.GetHashCode ( );

	public override string ToString ( ) => $"MEM[base: {Name}, offset: {Offset}]";
}