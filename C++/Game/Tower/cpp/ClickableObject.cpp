#include "../stdafx.h"
#include "../include/ClickableObject.h"
#include "../include/GameObject.h"

void ClickableObject::ProcessEvent(int x, int y, MouseState state)
{
	// Проверяем является ли текущий объект от производного GameObject
	GameObject *ths = dynamic_cast<GameObject *>(this);
	if (ths != nullptr )
	{
		// Обрабатываем события для текущего объекта
		SDL_Point pos = { x, y };
		SDL_Rect rect = {0, 0, ths->Size->cx, ths->Size->cy };
		if (SDL_PointInRect(&pos, &rect))
		{
			if (this->mouseOverObject)
			{
				if (state == mbtLeftDown || state == mbtRightDown)
				{
					this->MouseButtonDown();
					if (!OnMouseButtonDown.isNull()) this->OnMouseButtonDown();
				}
				if (state == mbtLeftUp || state == mbtRightUp)
				{
					this->Click();
					if (!OnClick.isNull()) this->OnClick();

					this->MouseButtonUp();
					if (!OnMouseButtonUp.isNull()) this->OnMouseButtonUp();
				}
				else
				{
					this->MouseMove();
					if (!OnMouseMove.isNull()) this->OnMouseMove();
				}
			}
			else
			{
				this->MouseOver();
				if (!OnMouseOver.isNull()) this->OnMouseOver();
				mouseOverObject = true;
			}
		}
		else
		{
			if (this->mouseOverObject)
			{
				mouseOverObject = false;
				this->MouseLeft();
				if (!OnMouseLeft.isNull()) this->OnMouseLeft();
			}

		}

		if (ths->objects.size() > 0)
		{
			// копируем список объектов - проблема в том, что в процессе обработки ProcessEvent могут изменится число обектов
			// и iter ++ упадет с ошибкой
			std::list<std::shared_ptr<GameObject>> objects(ths->objects);
			// Передаем события в дочерние объекты в порядке ZOrder (с конца очереди)
			auto iter = objects.crbegin();
			while(iter != objects.crend())
			{
				// Проверяем является ли объект наследником от ClickableObject
				auto object = (std::shared_ptr<GameObject>) *iter;
				ClickableObject * clickableObject = nullptr;
				clickableObject = dynamic_cast<ClickableObject *>(object.get());

				if (clickableObject != nullptr)
				{
					// Переводим в относительные координаты
					int nx = x - object->Position->x;
					int ny = y - object->Position->y;
					clickableObject->ProcessEvent(nx, ny, state);
				}
				iter++;
			}
		}
	}
};