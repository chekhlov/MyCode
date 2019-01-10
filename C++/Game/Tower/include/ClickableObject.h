#pragma once

#include "IClickableObject.h"

class ClickableObject: public IClickableObject
{
private:
protected:
	bool mouseOverObject = false;
public:
	ClickableObject() {};
	virtual ~ClickableObject() {};
	virtual void ProcessEvent(int x, int y, MouseState state);
	virtual void Click() override {};
	virtual void MouseMove() override {};
	virtual void MouseOver() override {};
	virtual void MouseLeft() override {};
	virtual void MouseButtonDown() override {};
	virtual void MouseButtonUp() override {};
};
