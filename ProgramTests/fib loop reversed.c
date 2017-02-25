#include <stdio.h>
#include <string.h>

unsigned long fib ( unsigned long n )
{
  int a = 1, b = 1, c;
  for ( int j = n; j != 0; --j )
  {
    c = a + b;
    a = b;
    b = c;
  }
  return c;
}

int main()
{
    int n = 20;
    int result = fib ( n );
    printf ( "%d\n", result );
    return 0;
}
