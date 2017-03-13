#include <stdio.h>
#include <string.h>

int main ()
{
	int n = 20;

	int a = 1, b = 1, c;
	for (int j = 0; j != n; ++j)
	{
		c = a + b;
		a = b;
		b = c;
	}
	printf ("%d\n", c);
	return 0;
}