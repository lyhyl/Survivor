#pragma once
#include <lua.hpp>
#include <lualib.h>
#include <lauxlib.h>

template <typename T>
void LuaCallPushArg(lua_State *L, T v) { throw new exception("Unsupported argument"); }
template <>
void LuaCallPushArg<signed char>(lua_State *L, signed char v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<char>(lua_State *L, char v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<short>(lua_State *L, short v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<int>(lua_State *L, int v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<long>(lua_State *L, long v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<long long>(lua_State *L, long long v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<unsigned char>(lua_State *L, unsigned char v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<unsigned short>(lua_State *L, unsigned short v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<unsigned int>(lua_State *L, unsigned int v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<unsigned long>(lua_State *L, unsigned long v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<unsigned long long>(lua_State *L, unsigned long long v) { lua_pushinteger(L, v); }
template <>
void LuaCallPushArg<float>(lua_State *L, float v) { lua_pushnumber(L, v); }
template <>
void LuaCallPushArg<double>(lua_State *L, double v) { lua_pushnumber(L, v); }
template <>
void LuaCallPushArg<bool>(lua_State *L, bool v) { lua_pushboolean(L, v); }
template <>
void LuaCallPushArg<char*>(lua_State *L, char *v) { lua_pushstring(L, v); }
template <>
void LuaCallPushArg<const char*>(lua_State *L, const char *v) { lua_pushstring(L, v); }

template < typename... Ts >
void LuaCallPush(lua_State *, Ts&&...);
template <>
void LuaCallPush(lua_State *L) { }
template < typename T, typename... RestT >
void LuaCallPush(lua_State *L, T a, RestT&&... as)
{
	LuaCallPushArg(L, a);
	LuaCallPushArg(L, forward<RestT>(as)...);
}

template<typename T>
void LuaCallGetRet(lua_State *L, T& v) { throw new exception("Unsupported type"); }
template <>
void LuaCallGetRet<signed char>(lua_State *L, signed char& v) { v = (signed char)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<char>(lua_State *L, char& v) { v = (char)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<short>(lua_State *L, short& v) { v = (short)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<int>(lua_State *L, int& v) { v = (int)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<long>(lua_State *L, long& v) { v = (long)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<long long>(lua_State *L, long long& v) { v = (long long)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<unsigned char>(lua_State *L, unsigned char& v) { v = (unsigned char)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<unsigned short>(lua_State *L, unsigned short& v) { v = (unsigned short)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<unsigned int>(lua_State *L, unsigned int& v) { v = (unsigned int)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<unsigned long>(lua_State *L, unsigned long& v) { v = (unsigned long)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<unsigned long long>(lua_State *L, unsigned long long& v) { v = (unsigned long long)lua_tointeger(L, -1); }
template <>
void LuaCallGetRet<float>(lua_State *L, float& v) { v = (float)lua_tonumber(L, -1); }
template <>
void LuaCallGetRet<double>(lua_State *L, double& v) { v = (double)lua_tonumber(L, -1); }
template <>
void LuaCallGetRet<bool>(lua_State *L, bool& v) { v = lua_tointeger(L, -1) != 0; }
template <>
void LuaCallGetRet<char*>(lua_State *L, char*& v) { v = (char*)lua_tostring(L, -1); }
template <>
void LuaCallGetRet<const char*>(lua_State *L, const char*& v) { v = (const char*)lua_tostring(L, -1); }

template < typename... Ts >
void LuaCallGet(lua_State *, Ts&&...);
template <>
void LuaCallGet(lua_State *L) { }
template < typename T, typename... RestT >
void LuaCallGet(lua_State *L, T& a, RestT&... as)
{
	LuaCallGetRet(L, a);
	lua_pop(L, 1);
	LuaCallGet(L, forward<RestT>(as)...);
}

template < typename... Ts >
class LuaCall;
template < typename T, typename... RestT >
class LuaCall < T, RestT... >
{
private:
	lua_State *_L;
	void *_code;
	int _codesz;
	const char *_name;
	int _retc;
	static const size_t argc = 1 + sizeof...(RestT);
public:
	LuaCall(lua_State *L, void *bytecode, int codesz, int retc, const char *name = "lua call")
		:_L(L), _code(bytecode), _codesz(codesz), _retc(retc), _name(name) { }
	void operator()(T a, RestT&&... as)
	{
		luaL_loadbuffer(_L, (const char*)_code, _codesz, _name);
		LuaCallPush(_L, a, forward<RestT>(as)...);
		lua_call(_L, argc, _retc);
	}
	template < typename U, typename... RestU >
	void get(U &u, RestU&... us)
	{
		if (sizeof...(RestU)+1 > _retc)
			throw new exception("No enough return values");
		LuaCallGet(_L, u, forward<RestU>(us)...);
	}
};