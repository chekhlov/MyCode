#include "../stdafx.h"
#include "../include/GameObject.h"
#include "../include/TextObject.h"
#include "../include/ButtonObject.h"

GameObject::GameObject()
{
	owner = nullptr;
	resource = nullptr;
	Size(this);
	Resource(this);
};

GameObject::GameObject(std::shared_ptr<GameObject> owner, int ZOrder) : owner(owner),  ZOrder(ZOrder)
{
	Size(this);
	Resource(this);
	resource = std::shared_ptr<IGraphicResource>(new GraphicResource(owner->resource));
};

GameObject::GameObject(std::shared_ptr<GameObject> owner, int cx, int cy, int ZOrder) : owner(owner), ZOrder(ZOrder)
{
	Size(this);
	Resource(this);
	resource = std::shared_ptr<IGraphicResource>(new GraphicResource(owner->resource, cx, cy));
};

GameObject::GameObject(std::shared_ptr<GameObject> owner, std::shared_ptr<IGraphicResource> resource, int ZOrder) : owner(owner), ZOrder(ZOrder)
{
	Size(this);
	Resource(this);
	this->resource = resource;
};


GameObject::~GameObject()
{
	objects.clear();
};

int GameObject::GetZOrder()
{
	return ZOrder;
};

std::shared_ptr<GameObject> GameObject::GetOwner()
{
	return owner;
}

void GameObject::AddObject(std::shared_ptr<GameObject> object, int ZOrder)
{
	if (object == nullptr) return;

	if (ZOrder != 0) object->ZOrder = ZOrder;
	
	// Проверяме наличие уже объекта
	if (ExistsObject(object)) return;

	// Сортируем объекты по ZOrder
	auto iter = objects.cbegin();
	while (iter != objects.cend())
	{
		if (iter->get()->ZOrder < object->ZOrder) break;
		++iter;
	}
	if (iter != objects.cend())
		objects.emplace(iter, object);
	else
		objects.push_back(object);
}


std::shared_ptr<IGraphicResource> GameObject::LoadResource(std::shared_ptr<IGraphicResource> owner, std::string fileName)
{
	std::shared_ptr<IGraphicResource>res(new GraphicResource(owner));
	res->LoadResource(fileName);
	return res;
}

std::shared_ptr<IGraphicResource> GameObject::LoadResource(std::string fileName)
{
	resource = std::shared_ptr<IGraphicResource>(new GraphicResource(owner->resource));
	resource->LoadResource(fileName);
	return resource;
}

std::shared_ptr<GameObject> GameObject::AddObject(int ZOrder)
{
	std::shared_ptr<GameObject>object(new GameObject(std::static_pointer_cast<GameObject>(shared_from_this())));
	AddObject(object, ZOrder);
	return object;
}

std::shared_ptr<GameObject> GameObject::AddObject(std::string resource, int ZOrder)
{
	std::shared_ptr<GameObject>object(new GameObject(std::static_pointer_cast<GameObject>(shared_from_this())));
	object->resource->LoadResource(resource);
	AddObject(object, ZOrder);
	return object;
}

void GameObject::DeleteObjects()
{
	objects.clear();
}

bool GameObject::ExistsObject(std::shared_ptr<GameObject> object)
{
	// Проверяме наличие уже объекта
	auto iter = objects.cbegin();
	while (iter != objects.cend())
		if (*iter++ == object) return true;

	return false;
}

void GameObject::DeleteObject(std::shared_ptr<GameObject> object)
{
	// Проверяме наличие уже объекта
	auto iter = objects.cbegin();
	while (iter != objects.cend())
	{
		if (*iter == object) 
		{
			objects.erase(iter);
			break;
		}
		iter++;
	}
}

std::shared_ptr<TextObject> GameObject::AddTextObject(int ZOrder)
{
	std::shared_ptr<TextObject>object(new TextObject(std::static_pointer_cast<GameObject>(shared_from_this())));
	AddObject(object, ZOrder);
	return object;
}

std::shared_ptr<ButtonObject> GameObject::AddButtonObject(std::string resource, int ZOrder)
{
	std::shared_ptr<ButtonObject>object(new ButtonObject(std::static_pointer_cast<GameObject>(shared_from_this())));
	object->AddFrame(resource);
	AddObject(object, ZOrder);
	return object;

}
std::shared_ptr<IGraphicResource> GameObject::GetResource()
{
	return resource;
}

std::shared_ptr<IGraphicResource> GameObject::SetResource(std::shared_ptr<IGraphicResource> const & object)
{
	resource = object;
	return resource;
}

SIZE GameObject::GetSize()
{
	SIZE size = {0, 0};
	// Используем GetSize - потому, что может быть и GraphicEngine
	if (resource != nullptr) size = std::static_pointer_cast<IGraphicResource>(resource)->GetSize();
	return size;
}

bool GameObject::Visible()
{
	if (owner == nullptr) return true; // у GameScreen - нет родителя

	SIZE osize = owner->GetSize();
	SDL_Rect rowner = { 0, 0, osize.cx, osize.cy };
	SDL_Rect rcurrent = { Position->x, Position->y, Size->cx, Size->cy };
	return SDL_HasIntersection(&rowner, &rcurrent);
}

void GameObject::Draw()
{
	// Если на поверхность не попадает, то отображать нечего
	if (!Visible()) return;
	
	if (objects.size() > 0)
	{ 
		resource->Clear();

		// отрисовываем все дочерние объекты
		for (auto object : objects)
			object->Draw();
	}

	// отрисовываем себя на родителе
	resource->Draw(Position->x, Position->y);
}
