using UnityEngine;
using System.Collections;

public static class MCoroutineHelper {

    public static MCoroutine StartCoroutine (this MCoroutine _mCoroutine, MonoBehaviour _caller, IEnumerator _coroutine) {

        return MCoroutine.StartCoroutine(_caller,_mCoroutine,_coroutine);

    }

}

public class MCoroutine {

    public bool running { get; protected set; }
    public bool paused;

    // ---

    protected IEnumerator wrapper;
    protected MonoBehaviour caller;

    // ---

    public static MCoroutine StartCoroutine (MonoBehaviour _caller, MCoroutine _mCoroutine, IEnumerator _coroutine) {

        if (_mCoroutine == null) _mCoroutine = new MCoroutine(_caller);
        _mCoroutine.Start(_coroutine);
        return _mCoroutine;

    }

    public MCoroutine (MonoBehaviour _caller) {
        caller = _caller;
    }

    public void Start (IEnumerator _coroutine) {

        Stop();

        // ---

        running = true;
        paused = false;

        wrapper = WrapperCoroutine(_coroutine);
        caller.StartCoroutine(wrapper);

    }

    IEnumerator WrapperCoroutine (IEnumerator _coroutine) {

        if (_coroutine == null) yield break;

        // yield return null;

        while (running) {

            if (paused) {

                yield return null;
            
            } else {

                if (caller != null && _coroutine.MoveNext()) {
                    yield return _coroutine.Current;
                } else {
                    running = false;
                }

            }
        }

        // --- end
    }

    public void Stop () {

        if (!running) return;

        running = false;
        caller.StopCoroutine(wrapper);

    }

}