using System.Text.RegularExpressions;

#pragma warning disable CS1591

public class GArrayDereference : GVar
{
	public string Offset { get; set; }

	public GArrayDereference ( string str )
	{
		string pattern = @"MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]";
		var match = Regex.Match ( str, pattern );
		Name = match.Groups["name"].Value;
		Offset = match.Groups["offset"].Value;
	}

	public static bool Matches ( string str )
	{
		return Regex.IsMatch ( str, @"^MEM\[base: (?<name>[\w\.]+), offset: (?<offset>\w+)]$" );
	}

	public override int GetHashCode ( )
	{
		return Name.GetHashCode ( );
	}

	public override string ToString ( )
	{
		return $"MEM[base: {Name}, offset: {Offset}]";
	}
}