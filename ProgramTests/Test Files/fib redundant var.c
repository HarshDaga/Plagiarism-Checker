#include <stdio.h>
#include <string.h>

unsigned long fib (unsigned long n)
{
	int a = 1, b = 1, c, d = 1, p = 0;
	for (int j = 0; j != n; ++j)
	{
		c = a + b;
		p = d + 5 & 6 + c - a^b;
		a = b;
		b = c;
		d = p * 5 + c + a;
	}
	return c;
}

int main ()
{
	int n = 20;
	int result = fib (n);
	printf ("%d\n", result);
	return 0;
}