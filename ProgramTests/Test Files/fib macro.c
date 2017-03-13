#include <stdio.h>
#include <string.h>


#define LOOP(i, n) for (int (i) = 0; (i) != (n); ++(i))

unsigned long fib (unsigned long n)
{
	int a = 1, b = 1, c;
	LOOP (j, n)
	{
		c = a + b;
		a = b;
		b = c;
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