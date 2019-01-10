#pragma once

#define interface struct

template <class T>
void swap_ptr(T * t1, T * t2)
{
	T *tmp = t1;
	t1 = t2;
	t2 = tmp;
}