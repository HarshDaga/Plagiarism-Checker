using System.Text.RegularExpressions;

#pragma warning disable CS1591

public class GArrayDereference : GVar
{
	public static readonly string pattern = @"^MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]$";

	public string Offset { get; set; }

	public GArrayDereference ( string str )
	{
		var match = Regex.Match ( str, pattern );
		Name = match.Groups["name"].Value;
		Offset = match.Groups["offset"].Value;
	}

	public static bool Matches ( string str ) => Regex.IsMatch ( str, pattern );

	public override int GetHashCode ( ) => Name.GetHashCode ( );

	public override string ToString ( ) => $"MEM[base: {Name}, offset: {Offset}]";
}