#include "../stdafx.h"
#include "../include/GraphicEngine.h"

// Создание ресурса из существующей текстуры
GraphicResource::GraphicResource(std::shared_ptr<IGraphicResource> owner, SDL_Texture *texture, int cx, int cy) 
{
	Size(this); // Инициализируем property
	this->owner = owner;
	if (!owner.get())
		throw GameException("GraphicResource::GraphicResource Попытка создания непривязанного объекта");

	size = owner->GetSize();

	this->engine = owner->GetEngine();
	if (!this->engine.get())
		throw GameException("GraphicResource::GraphicResource Неудалось получить ссылку на IGameEngine ");

	if (!texture)
		throw GameException("GraphicResource::GraphicResource Попытка создания пустого объекта");

	size = { cx, cy };
	this->resource = texture;
}

// Создание ресурса по размерам
GraphicResource::GraphicResource(std::shared_ptr<IGraphicResource> owner, int cx, int cy) 
{
	Size(this); // Инициализируем property
	this->owner = owner;
	if (!owner.get())
		throw GameException("GraphicResource::GraphicResource Попытка создания непривязанного объекта");

	size = owner->GetSize();

	this->engine = owner->GetEngine();
	if (!this->engine.get())
		throw GameException("GraphicResource::GraphicResource Неудалось получить ссылку на IGameEngine ");

	if (cx <= 0 || cy <= 0)
		size = owner->GetSize();
	else
		size = { cx, cy };

	auto render = ((GraphicEngine *)engine.get())->render;

	// создаем текстуру
	auto texture = SDL_CreateTexture(render, SDL_PIXELFORMAT_RGBA8888, SDL_TEXTUREACCESS_TARGET, size.cx, size.cy);

	if (!texture)
		throw GameException("GraphicResource::GraphicResource неудалось создать объект");

	this->resource = texture;
}


GraphicResource::~GraphicResource()
{	
	Free();
}

void GraphicResource::Free()
{
//	if (engine) ((GraphicEngine *)engine.get())->FreeResource(this); // Это если мы бы использовали IGraphicEngine
	if (!resource) return;

	SDL_DestroyTexture(resource);
	resource = nullptr;

}

SIZE GraphicResource::GetSize()
{
	return size;
}

std::shared_ptr<IGraphicEngine> GraphicResource::GetEngine()
{
	return engine;
}

void GraphicResource::Clear()
{
	auto render = ((GraphicEngine *)engine.get())->render;

	SDL_SetRenderTarget(render, this->resource);
	SDL_SetRenderDrawColor(render, 0xA0, 0xA0, 0xA0, 0x00);
	SDL_RenderClear(render);
	SDL_SetRenderTarget(render, NULL);
}

void GraphicResource::Draw(SDL_Rect *dstRect)
{
	if (owner) owner->Draw(std::static_pointer_cast<IGraphicResource>(shared_from_this()), dstRect);
}

void GraphicResource::Draw(int x, int y, int cx, int cy)
{
	SDL_Rect dstRect = { x, y, cx, cy };
	Draw(&dstRect);
}

void GraphicResource::Draw(IGraphicResource *object, SDL_Rect *dstRect, SDL_Rect *srcRect)
{
	auto resource = (GraphicResource *)object;
	if (!resource || !dstRect) return;

	SIZE size = object->GetSize();

	if (dstRect->w <= 0) dstRect->w = size.cx;
	if (dstRect->h <= 0) dstRect->h = size.cy;

	SDL_Rect sRect = { 0, 0, dstRect->w, dstRect->h };
	srcRect = SDL_RectEmpty(srcRect) ? &sRect : srcRect;
	
	auto render = ((GraphicEngine *)engine.get())->render;

	SDL_SetRenderTarget(render, this->resource);
	SDL_RenderCopy(render, resource->resource, srcRect, dstRect);
	SDL_SetRenderTarget(render, NULL);

}

void GraphicResource::Draw(std::shared_ptr<IGraphicResource> object, SDL_Rect *dstRect, SDL_Rect *srcRect)
{
	Draw(object.get(), dstRect, srcRect);
}

void GraphicResource::Draw(std::shared_ptr<IGraphicResource> object, int x, int y, int cx, int cy)
{
	SDL_Rect dRect = { x, y, cx, cy };
	Draw(object.get(), &dRect, NULL);
}

std::shared_ptr<IGraphicResource> GraphicResource::GetOwner()
{
	return owner;
}

std::shared_ptr<IGraphicResource> GraphicResource::GetResource()
{
	if (!resource)
		throw GameException("GameObject::GetSprite У объекта нет ресурсов");

	return shared_from_this();
}

void GraphicResource::SetResource(std::shared_ptr<IGraphicResource> object)
{
	Free();
	auto obj = std::static_pointer_cast<GraphicResource>(object);
	resource = obj->resource;
	Size = obj->Size;
	obj->resource = nullptr;
}

void GraphicResource::LoadResource(std::string fileName)
{
	Free();
	SDL_Surface *img = IMG_Load(fileName.c_str());
	if (img == nullptr)
		throw GameException(fmt::format("GraphicResource::LoadResource Неудалось загрузить ресурсы: {0}", IMG_GetError()));

	auto render = ((GraphicEngine *)engine.get())->render;

	resource = SDL_CreateTextureFromSurface(render, img);
	if (resource == nullptr)
		throw GameException(fmt::format("GraphicResource::LoadResource Ошибка создания текстуры: {0}", IMG_GetError()));

	size = { img->w, img->h };

	SDL_FreeSurface(img);
}

