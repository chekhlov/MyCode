#pragma once

#include "windows.h"
#include "interface.h"
#include "IGraphicEngine.h"

class GraphicResource : public IGraphicResource
{
private:
protected:
	long uid = 0;			// Уникальный идентификатор ресурса (неиспользуется)
	SIZE size = {0, 0};
	std::shared_ptr<IGraphicEngine> engine = nullptr;
	std::shared_ptr<IGraphicResource> owner = nullptr;
	SDL_Texture *resource = nullptr;

	virtual void Draw(IGraphicResource *object, SDL_Rect *dstRect = NULL, SDL_Rect *srcRect = NULL);
public:
	GraphicResource(std::shared_ptr<IGraphicResource> owner, SDL_Texture *texture, int cx, int cy);
	GraphicResource(std::shared_ptr<IGraphicResource> owner, int cx = 0, int cy = 0);

	virtual std::shared_ptr<IGraphicResource> GetResource();
 	virtual void SetResource(std::shared_ptr<IGraphicResource> object);

	ROProperty<SIZE, IGraphicResource, &GraphicResource::GetSize> Size;


	virtual ~GraphicResource();
	virtual SIZE GetSize() override;
	virtual std::shared_ptr<IGraphicResource> GetOwner() override;
	virtual std::shared_ptr<IGraphicEngine> GetEngine() override;
	virtual void Free() override;
	virtual void Clear() override;
	// Рисовании на родителе
	virtual void Draw(int x, int y, int cx = 0, int cy = 0) override;
	virtual void Draw(SDL_Rect *dstRect = NULL) override;
	// Рисование на себе
	virtual void Draw(std::shared_ptr<IGraphicResource> object, SDL_Rect *dstRect = NULL, SDL_Rect *srcRect = NULL) override;
	virtual void Draw(std::shared_ptr<IGraphicResource> object, int x, int y, int cx = 0, int cy = 0) override;

	virtual void LoadResource(std::string fileName) override;
	friend class GraphicEngine;
	friend class GameObject;
	friend class TextObject;
	friend class AnimateGameObject;
};
