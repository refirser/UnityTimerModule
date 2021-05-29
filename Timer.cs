using PlayFab.AdminModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Timer : MonoBehaviour
{
	#region 싱글톤
	private static Timer instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else {
			Destroy(this);
		}
	}

	public static Timer Instance
	{
		get { return instance; }
		set { instance = value; }
	}
	#endregion

	#region 타이머 체크 업데이트
	private void Update()
	{
		if (timer_time.Count > 0) {
			List<string> keys = new List<string>(timer_time.Keys);
			foreach (string key in keys) {
				if (timer_time.ContainsKey(key) && timer_time[key].Count <= 0)
				{
					timer_time.Remove(key);
					timer_time_copy.Remove(key);

					if (timer_func[key] == null)
					{
						Debug.LogError("타이머 함수가 존재하지 않습니다. 제거 불가");
					}
					else {
						timer_func.Remove(key);
					}
					
					continue;
				}

				List<int> indexs = new List<int>(timer_time[key].Keys);
				foreach (int index in indexs)
				{
					
					if (timer_time[key][index] <= 0f)
					{
						Debug.Log(timer_time_copy[key][index]);
						timer_time[key][index] = timer_time_copy[key][index];

						timer_reap[key][index] -= 1;

						if (timer_func[key] == null || timer_func[key][index] == null)
						{
							Debug.LogError("타이머 함수가 존재하지 않습니다. 실행 불가");

							timer_reap[key].Remove(index);
							timer_time[key].Remove(index);
							timer_time_copy[key].Remove(index);

							continue;
						}
						else
						{
							timer_func[key][index]();
						}

						if (timer_reap[key][index] <= 0f) {
							timer_reap[key].Remove(index);
							timer_time[key].Remove(index);
							timer_func[key].Remove(index);
							timer_time_copy[key].Remove(index);
						}

						
						continue;
					}

					

					timer_time[key][index] -= Time.deltaTime;
				}
			}
		}

		if (s_timer_time.Count > 0)
		{
			for (int i = 0; i < s_timer_time.Count; i++) {
				if (s_timer_time[i] <= 0) {
					if (s_timer_func[i] == null)
					{
						Debug.LogError("타이머는 존재하나 실행 함수가 존재하지 않습니다.");
					}
					else {
						s_timer_func[i]();
					}

					s_timer_func.RemoveAt(i);
					s_timer_time.RemoveAt(i);

					continue;
				}

				s_timer_time[i] -= Time.deltaTime;
			}
		}
	}
	#endregion

	#region 타이머 생성
	private Dictionary<string, Dictionary<int, float>> timer_time = new Dictionary<string, Dictionary<int, float>>();
	private List<float> s_timer_time = new List<float>();

	private Dictionary<string, Dictionary<int, float>> timer_time_copy = new Dictionary<string, Dictionary<int, float>>();

	private Dictionary<string, Dictionary<int, int>> timer_reap = new Dictionary<string, Dictionary<int, int>>();

	private Dictionary<string, Dictionary<int, Action>> timer_func = new Dictionary<string, Dictionary<int, Action>>();
	private List<Action> s_timer_func = new List<Action>();

	public void Create(string name, float time, int reap, Action func) {

		if (!timer_time.ContainsKey(name) || timer_time[name].Count <= 0)
		{
			Dictionary<int, float> dict = new Dictionary<int, float>();
			Dictionary<int, int> reap_dict = new Dictionary<int, int>();
			Dictionary<int, Action> func_dict = new Dictionary<int, Action>();

			dict.Add(0, time);
			func_dict.Add(0, func);
			reap_dict.Add(0, reap);

			timer_time.Add(name, dict);
			timer_func.Add(name, func_dict);
			timer_reap.Add(name, reap_dict);

			timer_time_copy.Add(name, new Dictionary<int, float>(dict));
		}
		else
		{
			timer_time[name].Add(timer_time[name].Count, time);
			timer_func[name].Add(timer_func[name].Count, func);
			timer_reap[name].Add(timer_reap[name].Count, reap);

			timer_time_copy[name].Add(timer_time_copy[name].Count, time);
		}
	}

	public void Simple(float time, Action func) {
		s_timer_time.Add(time);
		s_timer_func.Add(func);
	}
	#endregion

	#region 타이머 존재 확인 
	public bool Exists(string name) {
		return timer_time.ContainsKey(name);
	}

	public bool ExistsByIndex(string name, int index)
	{
		if (timer_time.ContainsKey(name)) {
			return timer_time[name].ContainsKey(index);
		}
		return timer_time.ContainsKey(name);
	}
	#endregion

	#region 타이머 남은 시간 확인
	public float TimeLeft(string name) {
		if (timer_time.ContainsKey(name))
		{
			if (timer_time[name].ContainsKey(0))
			{
				return timer_time[name][0];
			}
		}

		return 0f;
	}

	public float TimeLeftByIndex(string name, int index)
	{
		if (timer_time.ContainsKey(name))
		{
			if (timer_time[name].ContainsKey(index))
			{
				return timer_time[name][index];
			}
		}

		return 0f;
	}
	#endregion

	#region 타이머 반복 카운트 확인
	public int RepsLeft(string name) {
		if (timer_time.ContainsKey(name) && timer_reap.ContainsKey(name)) {
			if (timer_reap[name].ContainsKey(0)) {
				return timer_reap[name][0];
			}
		}
		
		return 0;
	}

	public int RepsLeftByIndex(string name, int index)
	{
		if (timer_time.ContainsKey(name) && timer_reap.ContainsKey(name))
		{
			if (timer_reap[name].ContainsKey(index))
			{
				return timer_reap[name][index];
			}
		}

		return 0;
	}
	#endregion
}
