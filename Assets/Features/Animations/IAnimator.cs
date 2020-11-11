using UnityEngine;

namespace Features.Animations {
/// <summary>
/// Copy pasted this from the decompiled unity animator so that I could auto-generate all of these function
/// and delegate them to the animator field
/// </summary>
interface IAnimator {
    float GetFloat(string name);

    /// <summary>
    ///   <para>Returns the value of the given float parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>The value of the parameter.</para>
    /// </returns>
    float GetFloat(int id);

    /// <summary>
    ///   <para>Send float values to the Animator to affect transitions.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    /// <param name="dampTime">The damper total time.</param>
    /// <param name="deltaTime">The delta time to give to the damper.</param>
    void SetFloat(string name, float value);

    /// <summary>
    ///   <para>Send float values to the Animator to affect transitions.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    /// <param name="dampTime">The damper total time.</param>
    /// <param name="deltaTime">The delta time to give to the damper.</param>
    void SetFloat(string name, float value, float dampTime, float deltaTime);

    /// <summary>
    ///   <para>Send float values to the Animator to affect transitions.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    /// <param name="dampTime">The damper total time.</param>
    /// <param name="deltaTime">The delta time to give to the damper.</param>
    void SetFloat(int id, float value);

    /// <summary>
    ///   <para>Send float values to the Animator to affect transitions.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    /// <param name="dampTime">The damper total time.</param>
    /// <param name="deltaTime">The delta time to give to the damper.</param>
    void SetFloat(int id, float value, float dampTime, float deltaTime);

    /// <summary>
    ///   <para>Returns the value of the given boolean parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>The value of the parameter.</para>
    /// </returns>
    bool GetBool(string name);

    /// <summary>
    ///   <para>Returns the value of the given boolean parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>The value of the parameter.</para>
    /// </returns>
    bool GetBool(int id);

    /// <summary>
    ///   <para>Sets the value of the given boolean parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    void SetBool(string name, bool value);

    /// <summary>
    ///   <para>Sets the value of the given boolean parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    void SetBool(int id, bool value);

    /// <summary>
    ///   <para>Returns the value of the given integer parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>The value of the parameter.</para>
    /// </returns>
    int GetInteger(string name);

    /// <summary>
    ///   <para>Returns the value of the given integer parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>The value of the parameter.</para>
    /// </returns>
    int GetInteger(int id);

    /// <summary>
    ///   <para>Sets the value of the given integer parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    void SetInteger(string name, int value);

    /// <summary>
    ///   <para>Sets the value of the given integer parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <param name="value">The new parameter value.</param>
    void SetInteger(int id, int value);

    /// <summary>
    ///   <para>Sets the value of the given trigger parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    void SetTrigger(string name);

    /// <summary>
    ///   <para>Sets the value of the given trigger parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    void SetTrigger(int id);

    /// <summary>
    ///   <para>Resets the value of the given trigger parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    void ResetTrigger(string name);

    /// <summary>
    ///   <para>Resets the value of the given trigger parameter.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    void ResetTrigger(int id);

    /// <summary>
    ///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
    /// </returns>
    bool IsParameterControlledByCurve(string name);

    /// <summary>
    ///   <para>Returns true if the parameter is controlled by a curve, false otherwise.</para>
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="id">The parameter ID.</param>
    /// <returns>
    ///   <para>True if the parameter is controlled by a curve, false otherwise.</para>
    /// </returns>
    bool IsParameterControlledByCurve(int id);
}
}