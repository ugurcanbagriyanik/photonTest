using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;

public class PlayfabHelper
{
    static Dictionary<string, UserDataRecord> _userData;
    static bool isGettingUserData = false;

    public static void SaveData(
        Dictionary<string, string> Data,
        Action<UpdateUserDataResult> onSuccess,
        Action<PlayFabError> onFail)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = Data
        },
        res => {
            //Updating Local Copy of User Data
            if (_userData != null)
            foreach (var key in Data.Keys)
            {
                //Creating a UserDataRecord with the updated data that will be stored in the dictionary
                UserDataRecord Value = new () { Value = Data[key] };

                if (_userData.ContainsKey(key)) _userData[key] = Value;
                else _userData.Add(key, Value);
            }

            onSuccess(res);
            },
        onFail);
    }
    public static void TryIncrementValue(
        string key,
        int amount,
        Action<UpdateUserDataResult> onSuccess = null,
        Action<PlayFabError> onFail = null)
    {
        //Setting Default Callbacks
        if(onSuccess == null) onSuccess = res => Debug.Log($"Successfully Incremented {key} In PlayFab");
        if(onFail == null) onFail = err => Debug.Log($"PlayFab Error: {err.GenerateErrorReport()}");

        TryGetData<int>(key,
            value => SaveData(new() { { key, (value + amount).ToString() } }, onSuccess, onFail),
            error => SaveData(new() { { key, amount.ToString() } }, onSuccess, onFail)
        );
    }
    public static async void GetUserData(
        Action<GetUserDataResult> onSuccess,
        Action<PlayFabError> onFail)
    {
        while (isGettingUserData)
            await Task.Delay(100);

        if (_userData != null)
        {
            onSuccess(new GetUserDataResult() { Data = _userData });
            return;
        }

        isGettingUserData = true; //-> Causes crash
        PlayFabClientAPI.GetUserData(new (),
            result => {
                _userData = result.Data;
                isGettingUserData = false;
                onSuccess(result);
            },
            onFailResult =>
            {
                isGettingUserData = false;
                onFail(onFailResult);
            }
        );
    }
    public static void TryGetData<T>(
        string Key,
        Action<T> onSuccess, 
        Action<PlayFabError> onFail)
    {
        try
        {
            if(_userData != null)
            {
                if (_userData.ContainsKey(Key))
                    onSuccess((T)Convert.ChangeType(_userData[Key].Value, typeof(T)));
                else onFail(new());
                return;
            }

            PlayFabClientAPI.GetUserData(new (),
                GetResult => {
                    _userData = GetResult.Data;

                    if (_userData.ContainsKey(Key))
                        onSuccess((T)Convert.ChangeType(GetResult.Data[Key].Value, typeof(T)));
                    else onFail(new());
                    },
                Error => onFail(Error)
            );
        }
        catch (Exception e)
        {
            onFail(new ());
        }
    }
}