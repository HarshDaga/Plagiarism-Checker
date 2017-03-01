#include <stdio.h>
#include <string.h>

unsigned long fib ( unsigned long n )
{
	int x = 1, y = 1, z;
	for ( int j = 0; j != n; ++j )
	{
		z = x + y;
		x = y;
		y = z;
	}
	return z;
}

int main ( )
{
	int n = 20;
	int result = fib ( n );
	printf ( "%d\n", result );
	return 0;
}