#pragma once

#ifndef _BUTTONOBJECT_H_
#define _BUTTONOBJECT_H_
#include "windows.h"
#include "interface.h"
#include "AnimateGameObject.h"
#include "../helpers/property.h"
#include "ClickableObject.h"

class ButtonObject : public AnimateGameObject, public ClickableObject
{
public:
	enum ButtonState { btNone, btFocused, btActive, btDown };
	Property<bool> focused;
private:
	ButtonState state;
protected:
	virtual bool GetFocus();
	virtual bool SetFocus(bool const &size);
	virtual void SetState(ButtonState state);
	void SetDafaultState();
public:

	ButtonObject(std::shared_ptr<GameObject> owner, int ZOrder = 0);

	RWProperty<bool, ButtonObject, &ButtonObject::GetFocus, &ButtonObject::SetFocus> Focus;

	virtual void MouseOver() override;
	virtual void MouseLeft() override;
	virtual void MouseButtonDown() override;
	virtual void MouseButtonUp() override;

	virtual std::shared_ptr<IGraphicResource> operator[](ButtonState state);


};

#endif