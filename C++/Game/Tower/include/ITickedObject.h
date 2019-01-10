#pragma once

#include "interface.h"

interface ITickedObject
{	

	virtual ~ITickedObject() = 0 {};
	virtual void Tick(int tick) = 0;
	virtual void Init() = 0;
};
