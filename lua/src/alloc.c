#include "lua.h"
#include <stdlib.h>

void *alloc (void *ud, void *ptr, size_t osize, size_t nsize) 
{
	(void)ud;  
	(void)osize;  /* not used */

	if (nsize == 0) 
	{
		free(ptr);
		return NULL;
	}
	else
	{
		return realloc(ptr, nsize);
	}
}

LUA_API lua_Alloc getDefaultAllocator();