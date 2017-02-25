#include <stdio.h>
#include <string.h>

int main()
{
    int a, b, max;
	scanf ( "%d %d", &a, &b );
	if ( a < b )
		max = b;
	else
		max = a;
	printf ( "%d\n", max );
    return 0;
}
