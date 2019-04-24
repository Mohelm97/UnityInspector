# ![UnityInspector Logo](https://user-images.githubusercontent.com/1688821/56691454-aba9f980-66e8-11e9-90b6-4fdf714404b2.png)  UnityInspector

A simple tool for inspecting and editing managed Unity3d games in real-time, using a simple layout you can see game objects in the active scene, and you can click at any game object to see its components, and click at any component to see its members and edit values.

<p align="center">
<img alt="Screenshot" src="https://user-images.githubusercontent.com/1688821/56691759-6508cf00-66e9-11e9-89e8-db9563b4113f.png" />
</p>

## How to build
1. Clone the project.
2. Add UnityEngine.dll refrence to UnityInspector.Inejctor.
3. Tadaaa.

## How UnityInspector works
First I tried to build this using a direct memory read and write method and it works kinda well (I'm laying), but using the previous method will leave a lot of work to do for each unity3d engine version (but it works on managed and il2cpp games).
So now this project is using the awesome [warbler/SharpMonoInjector](https://github.com/warbler/SharpMonoInjector) to inject a managed dll inside mono which communicates through TCP with the main program, well and some cool C# code.
<p align="center">
<img alt="A diagram just to make it cool" src="https://user-images.githubusercontent.com/1688821/56693508-4dcbe080-66ed-11e9-9c70-ae679f9caa9f.png" />
</p>

## Todo
- [ ] Add data locks
- [ ] Some error handling
- [ ] Support nested objects
- [ ] Change GameObjects and components references
- [ ] Browse prefabs and other GameObjects
