// namespace DRG.Tests
// {
//     using System.Collections;
//     using NUnit.Framework;
//     using UnityEngine;
//     using UnityEngine.TestTools;
//     using Utils;
//     
//     /// <summary>
//     /// Those tests are not working in editor
//     /// </summary>
//     public class DebouncedExecutorUnityTests
//     {
//         private class MockLogger : DRG.Logs.ILogger
//         {
//             public void Log(string message) { }
//             public void LogWarning(string message) { }
//             public void LogError(string message) { }
//             public void LogException(System.Exception exception) { }
//         }
//
//         private class TestMono : MonoBehaviour
//         {
//             public int callCount;
//             public void Increment() => callCount++;
//         }
//
//         [UnityTest]
//         public IEnumerator Execute_ActionRunsAfterCooldown()
//         {
//             var go = new GameObject("test");
//             var mono = go.AddComponent<TestMono>();
//             var logger = new MockLogger();
//             var executor = new DebouncedExecutorUnity(mono, logger);
//
//             executor.Execute(2, mono.Increment);
//
//             Assert.That(mono.callCount, Is.EqualTo(0));
//             yield return null; // frame 1
//             Assert.That(mono.callCount, Is.EqualTo(0));
//             yield return null; // frame 2
//             Assert.That(mono.callCount, Is.EqualTo(1));
//
//             Object.DestroyImmediate(go);
//         }
//
//         [UnityTest]
//         public IEnumerator Execute_BeforeCooldownResetsTimer()
//         {
//             var go = new GameObject("test");
//             var mono = go.AddComponent<TestMono>();
//             var logger = new MockLogger();
//             var executor = new DebouncedExecutorUnity(mono, logger);
//
//             executor.Execute(3, mono.Increment);
//             yield return null; // frame 1
//             executor.Execute(2, mono.Increment);
//             yield return null; // new frame 1
//             Assert.That(mono.callCount, Is.EqualTo(0));
//             yield return null; // new frame 2
//             Assert.That(mono.callCount, Is.EqualTo(1));
//
//             Object.DestroyImmediate(go);
//         }
//     }
// }
