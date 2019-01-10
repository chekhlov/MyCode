#pragma once

#include "interface.h"
#include "../helpers/delegate.h"

interface IClickableObject
{	
	enum MouseState { mbtLeftDown, mbtRightDown, mbtMove, mbtLeftUp, mbtRightUp};

	typedef SA::delegate<void()> delegate;

	delegate OnClick;
	delegate OnMouseMove;
	delegate OnMouseOver;
	delegate OnMouseLeft;
	delegate OnMouseButtonDown;
	delegate OnMouseButtonUp;

	virtual ~IClickableObject() = 0 {};
	virtual void Click() = 0;
	virtual void MouseButtonDown() = 0;
	virtual void MouseButtonUp() = 0;
	virtual void MouseMove() = 0;
	virtual void MouseOver() = 0;
	virtual void MouseLeft() = 0;

	virtual void ProcessEvent(int x, int y, MouseState state) = 0;
};
