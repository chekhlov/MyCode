// stdafx.h: включаемый файл для стандартных системных включаемых файлов
// или включаемых файлов для конкретного проекта, которые часто используются, но
// не часто изменяются
//

#pragma once

#include "targetver.h"

#define WIN32_LEAN_AND_MEAN             // Исключите редко используемые компоненты из заголовков Windows
// Файлы заголовков Windows:
#include <windows.h>

// Файлы заголовков C RunTime
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>
#include <string>
#include "libs/sdl2/include/SDL.h"
#include "libs/sdl2/include/SDL_image.h"
#include "libs/fmt/include/fmt/format.h"
#include "include/GameException.h"
#include "helpers/ShowMessage.h"
#include "helpers/stringConvert.h"
#include "helpers/property.h"

// #pragma comment(lib, "SDL2.lib")

