#pragma once
#pragma unmanaged
#include <jni.h>
#include <Windows.h>
#include "AIAdapter.h"

static char JavaVMOptionBuffer[MAX_PATH + 100] = "-Djava.class.path=.;";
static bool JavaInitialized = false;
static unsigned __int64 jvmRef = 0;
static JavaVM *jvm = nullptr;
static JNIEnv *env = nullptr;
static JavaVMOption options[3];

void InitJavaVM()
{
	typedef jint(JNICALL *pCreateJavaVM)(JavaVM **, void**, void *);

	TCHAR jpath[MAX_PATH] = { 0 };
	DWORD bufferSize = sizeof(jpath);
	RegGetValue(HKEY_LOCAL_MACHINE, __TEXT("SOFTWARE\\JavaSoft\\Java Runtime Environment\\1.8\\"),
		__TEXT("RuntimeLib"), RRF_RT_REG_SZ, 0, jpath, &bufferSize);

	HINSTANCE jvmDLL = LoadLibrary(jpath);
	if (jvmDLL == NULL)
	{
		MessageBox(0, __TEXT("Unable to init java vm"), __TEXT("Error"), 0);
		exit(1);
	}
	pCreateJavaVM CreateJavaVM = (pCreateJavaVM)GetProcAddress(jvmDLL, "JNI_CreateJavaVM");

	DWORD len = GetModuleFileNameA(NULL, &JavaVMOptionBuffer[20], MAX_PATH);
	char *pFileName = &JavaVMOptionBuffer[20 + len];
	while (*pFileName != '/' && *pFileName != '\\')
	{
		*pFileName = 0;
		pFileName--;
	}
	strcat_s(JavaVMOptionBuffer, "Heroes\\");

	options[0].optionString = JavaVMOptionBuffer;
	JavaVMInitArgs vm_args;
	vm_args.version = JNI_VERSION_1_8;
	vm_args.nOptions = 1;
	vm_args.options = options;
	vm_args.ignoreUnrecognized = JNI_TRUE;
	if (CreateJavaVM(&jvm, (void**)&env, &vm_args) == JNI_ERR)
	{
		MessageBox(0, __TEXT("Unable to init java vm"), __TEXT("Error"), 0);
		exit(1);
	}
	JavaInitialized = true;
}

class JavaImplWapper :AIAdapter
{
private:
	jobject jAdapter = nullptr;
	jclass jAdapterClass = nullptr;
	jmethodID jThinkMethodID = nullptr;
public:
	JavaImplWapper(wstring &file)
	{
		if (!JavaInitialized)
			InitJavaVM();
		wstring::size_type nb = file.find_last_of(__TEXT("\\/"));
		wstring::size_type ne = file.find_last_of(L'.');
		string className(file.begin() + nb + 1, file.begin() + ne);
		jAdapterClass = env->FindClass((className).c_str());
		if (jAdapterClass != 0)
		{
			jThinkMethodID = env->GetMethodID(jAdapterClass, "Test", "()I");
			jAdapter = env->AllocObject(jAdapterClass);
			jvmRef++;
		}
	}
	~JavaImplWapper()
	{
		jvmRef--;
		if (jvmRef == 0)
		{
			jvm->DestroyJavaVM();
			JavaInitialized = false;
			jvm = nullptr;
		}
	}
	virtual SCCompetitorAction Think(SCCompetitor *competitor, SCCompetitorVision vision)
	{
		JNIEnv *threadEnv = nullptr;
		jvm->AttachCurrentThread((void**)&threadEnv, options);
		jint r = threadEnv->CallIntMethod(jAdapter, jThinkMethodID);
		return{ 0 };
	}
};