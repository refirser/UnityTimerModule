# UnityTimerModule
Unity's Timer Module

You can use this follow Example :
//Timer Create 
//Name, Time, Reapeat, Function
Timer.Instance.Create("identify", 0.1F, 1, () => {
  Debug.Log("test timer");
}
)

//Simple Timer Create
//Time, Function
Timer.Instance.Simple(0.1F, () => {
  Debug.Log("test simple timer");
}
)
