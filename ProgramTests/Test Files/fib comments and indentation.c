#include <stdio.h>

#include <string.h>

#include<malloc.h>

#include<math.h>

#include<stdlib.h>

// Dummy comment
unsigned long fib(unsigned long n){

int a=1,b=1,c;
// Loop n times
for(int j=0;j!=n;++j){
c = a + b;		// c is current result
a = b;			// a is previous result
b = c;			// b is previous result stored for next computation
}
return c;
}

int main(){
int n=20;
int result=fib(n);
printf("%d\n",result);
return 0;
}
