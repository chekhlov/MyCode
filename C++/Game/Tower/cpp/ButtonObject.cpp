#include "../stdafx.h"
#include "../include/ButtonObject.h"

ButtonObject::ButtonObject(std::shared_ptr<GameObject> owner, int ZOrder) : AnimateGameObject(owner, ZOrder)
{
	Focus(this);
	focused = false;
//	resources.resize(4);
};

bool ButtonObject::GetFocus()
{
	return focused;
}
 bool ButtonObject::SetFocus(bool const &focus)
{
	focused = focus;
	SetState(focused ? btFocused : btNone);
	return focused;
}

 void ButtonObject::SetDafaultState()
 {
	 SetState(focused ? btFocused : mouseOverObject ? btActive : btNone);
 }

 void ButtonObject::SetState(ButtonState state)
{
	int n = (int) state;
	if (n >= Count - 1) n = Count - 1;
	SetFrame(n);
}

void ButtonObject::MouseLeft()
{
	SetDafaultState();
}

void ButtonObject::MouseOver()
{
	SetState(btActive);
}

void ButtonObject::MouseButtonDown()
{
	SetState(btDown);
}

void ButtonObject::MouseButtonUp()
{
	SetDafaultState();
}

 std::shared_ptr<IGraphicResource> ButtonObject::operator[](ButtonState state)
 {
	 if (resources.size() > state || state < 0)
		 throw GameException(fmt::format("ButtonObject::[] Индекс за пределами границы - {0}.", state));

	 return resources[state];
 }
