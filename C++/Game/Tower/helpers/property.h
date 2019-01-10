#pragma once

#ifndef _PROPERTY_H_
#define _PROPERTY_H_

#include <map> 
#include <initializer_list>

// Get from N1615 C++ Properties – a Library Solution

// a read-write property with data store and  
// automatically generated get/set functions. 
// this is what C++/CLI calls a trivial scalar property
// Some utility templates for emulating properties --
// preferring a library solution to a new language feature
// Each property has three sets of redundant acccessors :
// 1. function call syntax 
// 2. get() and set() functions 
// 3. overloaded operator = 
// a read-write property with data store and  
// automatically generated get/set functions. 
// this is what C++/CLI calls a trivial scalar property
template <class T>
class Property
{
	T data;
public:
	// access with function call syntax 
	Property() : data() { }
	T operator()() const
	{
		return data;
	}
	T operator()(T const & value)
	{
		data = value;
		return data;
	}
	// access with get()/set() syntax 
	T get() const
	{
		return data;
	}
	T set(T const & value)
	{
		data = value;
		return data;
	}
	// access with '=' sign 
	// in an industrial-strength library, 
	// specializations for appropriate types might choose to
	// add combined operators like +=, etc. 
	operator T() const
	{
		return data;
	}
	T operator = (T const & value)
	{
		data = value;
		return data;
	}

	//	operator T &() { return data; };

	T * operator-> () { return &data; };

	typedef T value_type;  // might be useful for template deductions
};


// a read-only property calling a user-defined getter
template <class T, class Object, typename T(Object	::*real_getter)()>
class ROProperty
{
	Object * my_object;
	T data;
public:
	// this function must be called by the containi	ng class, normally in a
	// constructor, to initialize the ROProperty soit knows where its
	// real implementation code can be found. obj is usually the containing
	// class, but need not be; it could be a special implementation object.
	void operator () (Object * obj)
	{
		my_object = obj;
	}
	// function call syntax
	T operator()() const
	{
		return (my_object->*real_getter)();
	}
	// get/set syntax 
	T get() const
	{
		return (my_object->*real_getter)();
	}
	void set(T const & value); // reserved but not implemented, per C++ / CLI
	// use on rhs of '=' 
	operator T() const
	{
		return (my_object->*real_getter)();
	}

//	operator T &() { return *(my_object->*real_getter)(); };

	const T * operator-> ()
	{
		data = (my_object->*real_getter)();
		return &data;
	};

	typedef T value_type;  // might be useful for template deductions
};


// a write-only property calling a user-defined setter
template <class T, class Object, typename T(Object::*real_setter)(T const &)>
class WOProperty
{
	Object * my_object;
	T data;
public:
	// this function must be called by the containing class, normally in
	// a constructor, to initialize the WOProperty so it knows where its
	// real implementation code can be found 
	void operator () (Object * obj)
	{
		my_object = obj;
	}
	// function call syntax    
	T operator()(T const & value)
	{
		return (my_object->*real_setter)(value);
	}
	// get/set syntax 
	T get() const;      // name reserved but not implemented per C++ / CLI
	T set(T const & value)
	{
		return (my_object->*real_setter)(value);
	}
	// access with '=' sign 
	T operator = (T const & value)
	{
		return (my_object->*real_setter)(value);
	}

//	operator T &() { return (my_object->*real_getter)(); };

	T * operator-> ()
	{
		data = (my_object->*real_getter)();
		return &data;
	};

	typedef T value_type;  // might be useful for template deductions
};

// a read-write property which invokes user-definedfunctions
template <class T, class Object, typename T(Object::*real_getter)(), typename T(Object::*real_setter)(T const &)>
class RWProperty
{
	Object * my_object;
	T data;
public:
	// this function must be called by the containing class, normally in a
	// constructor, to initialize the ROProperty soit knows where its
	// real implementation code can be found 
	void operator () (Object * obj)
	{
		my_object = obj;
	}
	// function call syntax    
	T operator()() const
	{
		return (my_object->*real_getter)();
	}
	T operator()(T const & value)
	{
		return (my_object->*real_setter)(value);
	}
	// get/set syntax 
	T get() const
	{
		return (my_object->*real_getter)();
	}
	T set(T const & value)
	{
		return (my_object->*real_setter)(value);
	}
	// access with '=' sign 
	operator T() const
	{
		return (my_object->*real_getter)();
	}
	T operator = (T const & value)
	{
		return (my_object->*real_setter)(value);
	}

//	operator T &() { return (my_object->*real_getter)(); };

	T * operator-> ()
	{
		data = (my_object->*real_getter)();
		return &data;
	};

	typedef T value_type;  // might be useful for template deductions
};

#endif