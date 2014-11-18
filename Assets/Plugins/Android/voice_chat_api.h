////////////////////////////////////////
///
/// @file voice_chat_api.h
/// @author Marcus Froeschl
/// @copyright (c) 2014 ageet Corporation
/// 

#ifndef _VOICECHAT_VOICECHATAPI_H__
#define _VOICECHAT_VOICECHATAPI_H__


/// Call State: Call is idle.
#define VC_CALL_STATE_IDLE (0)
/// Call State: Call is progressing, i.e. dialing or ringing, depending on the call's Direction.
#define VC_CALL_STATE_PROGRESSING (1)
/// Call State: Call is connected.
#define VC_CALL_STATE_CONNECTED (2)

/// Conference State: Conference is destroyed.
#define VC_CONFERENCE_STATE_DESTROYED (0)
/// Conference State: Conference is created.
#define VC_CONFERENCE_STATE_CREATED (1)

/// Conference Role: None
#define VC_CONFERENCE_ROLE_NONE (0)
/// Conference Role: Host
#define VC_CONFERENCE_ROLE_HOST (1)
/// Conference Role: Guest
#define VC_CONFERENCE_ROLE_GUEST (2)
/// Conference Role: Host or Guest. Role never actually takes this value. It is used internally for validation only.
#define VC_CONFERENCE_ROLE_HOST_OR_GUEST (3)

/// Log Level: Critical Error.
#define VC_LOG_LEVEL_CRITICAL_ERROR (0)
/// Log Level: Error.
#define VC_LOG_LEVEL_ERROR (1)
/// Log Level: Warning.
#define VC_LOG_LEVEL_WARNING (2)
/// Log Level: Info.
#define VC_LOG_LEVEL_INFO (3)
/// Log Level: Debug.
#define VC_LOG_LEVEL_DEBUG (4)
/// Log Level: Verbose
#define VC_LOG_LEVEL_VERBOSE (5)

/// Profile: Low Bandwidth.
#define VC_PROFILE_LOW_BANDWIDTH (0)
/// Profile: High Bandwidth.
#define VC_PROFILE_HIGH_BANDWIDTH (2)

/// Error: Unknown error.
#define VC_ERROR_UNKNOWN (-1)
/// No error (Success).
#define VC_SUCCESS (0)
/// Error: Could not create.
#define VC_ERROR_COULD_NOT_CREATE (1)
/// Error: Already exists.
#define VC_ERROR_ALREADY_EXISTS (2)
/// Error: Does not exist.
#define VC_ERROR_DOES_NOT_EXIST (3)
/// Error: Too Many.
#define VC_ERROR_TOO_MANY (4)
/// Error: Not enough.
#define VC_ERROR_NOT_ENOUGH (5)
/// Error: Invalid state.
#define VC_ERROR_INVALID_STATE (6)
/// Error: Invalid role.
#define VC_ERROR_INVALID_ROLE (7)
/// Error: Invalid parameter.
#define VC_ERROR_INVALID_PARAMETER (8)
/// Error: Engine Error
#define VC_ERROR_ENGINE_ERROR (9)
/// Error: Not implemented.
#define VC_ERROR_NOT_IMPLEMENTED (10)
/// Error: Out of memory.
#define VC_ERROR_OUT_OF_MEMORY (11)
/// Error: Busy.
#define VC_ERROR_BUSY (12)
/// Error: Timeout.
#define VC_ERROR_TIMEOUT (13)

/// Invalid ID for internal use
#define VC_INVALID_ID (-1)

#ifndef UNUSED
  /// For internal use
  #define UNUSED(arg) (void)arg
#endif  // LNK4221

extern "C" {
  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // VoiceChat Engine API
  //
  ////////////////////////////////////////////////////////////

  /// VCCallListener callback. 
  /// The SDK Platform Wrapper should implement this callback, and register a VCCallListener.
  /// @param group_id ID of the group.
  /// @param partner_id ID of the partner.
  /// @param call_state Call state. One of VC_CALL_STATE_IDLE, VC_CALL_STATE_PROGRESSING or VC_CALL_STATE_CONNECTED
  typedef void (*VCCallStateListener) (const char* group_id, const char* partner_id, int call_state);

  /// VCConferenceStateListener callback.
  /// The SDK Platform Wrapper should implement this callback, and register a VCConferenceStateListener.
  /// @param conference_state Conference state. One of VC_CONFERENCE_STATE_CREATED or VC_CONFERENCE_STATE_DESTROYED
  /// @param conference_role Our role in the conference when it was created / before it was destroyed. One of VC_CONFERENCE_ROLE_NONE, VC_CONFERENCE_ROLE_HOST or VC_CONFERENCE_ROLE_GUEST.
  /// @param log Log of the conference containing settings, conference info, call info, RTP info.
  typedef void (*VCConferenceStateListener) (int conference_state, int conference_role, const char* log);

  /// VCMayDestroyEngineListener callback.
  /// The SDK Platform Wrapper should implement this callback, and may call VCEngineDestroy() when it is invoked. 
  typedef void (*VCMayDestroyEngineListener) ();
  
  /// VCResultListener callback. 
  /// SDK Platform Wrapper should implement this callback, and register a VCResultListener.
  /// @param result_code Result code.
  /// @param result_message Result message.
  /// @param user_data User data.
  typedef void (*VCResultListener) (int result_code, const char* result_message, const void* user_data);

  /// VCErrorListener callback. 
  /// SDK Platform Wrapper should implement this callback, and register a VCErrorListener.
  /// @param error_code Error code.
  /// @param error_message Error message.
  typedef void (*VCErrorListener) (int error_code, const char* error_message);

  /// VCLogListener callback. 
  /// SDK Platform Wrapper should implement this callback, and register a VCLogListener.
  /// @param log_level Log level.
  /// @param log_message Log message.
  typedef void (*VCLogListener) (int log_level, const char *log_message);

  /// VCFresviiStartConferenceListener callback. 
  /// SDK Platform Wrapper MUST implement this callback, and call FGC Server's createConference() API in it.
  /// @param group_id Fresvii CoreSDK group id
  typedef int (*VCFresviiStartConferenceListener) (const char *group_id);

  /// Sets the VCCallStateListener of the Engine.
  /// A VCCallStateListener has to be registered to monitor ongoing calls.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCCallStateListener callback function that will receive the call state events.
  void VCEngineSetCallStateListener(VCCallStateListener listener);

  /// Sets the VCConferenceStateListener of the Engine.
  /// A VCConferenceStateListener has to be registered to the conference's state.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCConferenceStateListener callback function that will receive the conference state events.
  void VCEngineSetConferenceStateListener(VCConferenceStateListener listener);
  
  /// Sets the VCMayDestroyEngineListener of the Engine.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCMayDestroyEngineListener callback function that will receive the VCMayDestroyEngineListener event.
  void VCEngineSetMayDestroyEngineListener(VCMayDestroyEngineListener listener);
  
  /// Sets the VCErrorListener of the Engine.
  /// A VCErrorListener should be added in order to work with the Engine.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCErrorListener callback function that will receive the error events.
  void VCEngineSetErrorListener(VCErrorListener listener);

  /// Sets the VCResultListener of the Engine.
  /// A VCResultListener should be added in order to work with the Engine.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCResultListener callback function that will receive the result events.
  void VCEngineSetResultListener(VCResultListener listener);

  /// Sets the VCLogListener of the Engine.
  /// SDK Platform Wrapper should add a VCLogListener for debugging purposes.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// If this function is not called at all, a default VCLogListener callback will be invoked.
  /// @param listener Pointer to a VCLogListener callback function that will receive the log events. If NULL, no log will be output.
  void VCEngineSetLogListener(VCLogListener listener);

  /// Sets the VCFresviiStartConferenceListener of the Engine.
  /// SDK Platform Wrapper MUST add a VCFresviiStartConferenceListener.
  /// This function can be called safely only before VCEngineCreate() or after VCEngineDestroy().
  /// @param listener Pointer to a VCFresviiStartConferenceListener callback function that will receive the OnFresviiStartConference event.
  void VCEngineSetFresviiStartConferenceListener(VCFresviiStartConferenceListener listener);

  /// Sets the log level of the Engine.
  /// @param log_level Log level.
  void VCEngineSetLogLevel(int log_level);
  
  /// Creates the VoiceChat Engine.
  /// Must be called before invoking any VCConference... functions.
  /// @return Error code
  int VCEngineCreate();
  
  /// Destroys the VoiceChat engine.
  /// If VCEngineCreate() was called with VC_SUCCESS, this function MUST be called later to free resources.
  /// VCEngineDestroy() MUST be called from the wrapper's main thread.
  /// VCEngineDestroy() may be called when the VCMayDestroyEngineListener callback is invoked, or before the application exits.
  /// @return Error code
  int VCEngineDestroy();

  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // Internal SDK Wrapper API 
  //
  ////////////////////////////////////////////////////////////

  /// Updates the internal settings data.
  /// Updates the internal settings data which will be used when communicating with FGC server or SIP server.
  /// SDK Wrapper MUST call this function before VCHostCreateConference() and VCGuestJoinConference().
  /// This function is an internal function to be called by SDK wrapper ONLY.
  /// MUST NOT BE EXPOSED TO END USER.
  /// @param user_id SIP user ID.
  /// @param user_id Fresvii Group ID.
  /// @param sip_password SIP password.
  /// @param sip_domain SIP domain.
  /// @param stun_server_one STUN server one.
  /// @param stun_server_two STUN server two.
  /// @param sip_port SIP port
  /// @param profile Performance Profile. One of VC_PROFILE_LOW_BANDWIDTH or VC_PROFILE_HIGH_BANDWIDTH
  /// @return Error code.
  int VCInternalUpdateSettings(
      const char* user_id,
      const char* group_id,
      const char* sip_password,
      const char* sip_domain,
      const char* stun_server_one,
      const char* stun_server_two,
      int sip_port,
      int profile
    );

  /// Informs the engine about the result of FresviiStartConference function invoked by VCFresviiStartConferenceListener
  /// @param result_code Pass either VC_SUCCESS in case of SUCCESS or VC_ERROR_COULD_NOT_CREATE in case of error.
  void VCInternalFresviiStartConferenceResult(int result_code);

  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // VoiceChat Conference Host API
  //
  ////////////////////////////////////////////////////////////

  /// Creates a new conference.
  /// The result is returned asynchronously in VCResultListener
  /// @param group_id Fresvii CoreSDK group id
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCHostCreateConference(const char* group_id, const void* user_data);

  /// Adds a guest to a conference.
  /// The result is returned asynchronously in VCResultListener.
  /// The new call from the guest will be automatically mixed to the conference once connected..
  /// @param guest_id The Fresvii CoreSDK user ID of the guest to be added to the conference.
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCHostAddGuest(const char* guest_id, const void* user_data);

  /// Removes a guest from the conference.
  /// Also disconnects any SIP call to the conference guest.
  /// The result is returned asynchronously in VCResultListener.
  /// @param guest_id The Fresvii CoreSDK user ID of the guest to be removed from the conference.
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCHostRemoveGuest(const char* guest_id, const void* user_data);


  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // VoiceChat Conference Guest API
  //
  ////////////////////////////////////////////////////////////

  /// Joins a conference.
  /// The result is returned asynchronously in VCResultListener.
  /// @param group_id Fresvii CoreSDK group id of the conference to join
  /// @param host_id Fresvii CoreSDK user id of the conference host
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCGuestJoinConference(const char* group_id, const char* host_id, const void* user_data);


  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // VoiceChat Conference Shared API (both Host and Guest)
  //
  ////////////////////////////////////////////////////////////

  /// Leaves the conference.
  /// The result is returned asynchronously in VCResultListener.
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCConferenceLeave(const void* user_data);

  /// Mutes your mic in the conference.
  /// The result is returned asynchronously in VCResultListener.
  /// Use to implement push-to-talk.
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCConferenceMute(const void* user_data);

  /// Unmutes your mic in the conference.
  /// The result is returned asynchronously in VCResultListener.
  /// Use to implement push-to-talk.
  /// @param user_data User data that will be passed back to VCResultListener
  /// @return Error code.
  int VCConferenceUnmute(const void* user_data);
  
  /// Sets the volume for the conference.
  /// The result is returned asynchronously in VCResultListener.
  /// @param volume_microphone Microphone volume.
  /// @param volume_speaker Speaker volume.
  /// @return Error code.
  int VCConferenceSetVolume(float volume_microphone, float volume_speaker, const void* user_data);


  ////////////////////////////////////////////////////////////
  ////////////////////////////////////////////////////////////
  //
  // VoiceChat Generic API
  //
  ////////////////////////////////////////////////////////////

  /// Retrieves a human-readable error message for a given error code.
  /// @param error_code Error code.
  /// @return Human-readable error message.
  const char* VCGetErrorMessage(int error_code);
  
  /// Retrieves the group id of the conference
  /// @return group id.
  const char* VCGetConferenceGroupId();

  /// Retrieves the current role of the conference.
  /// @return Either VC_CONFERENCE_ROLE_NONE, VC_CONFERENCE_ROLE_HOST or VC_CONFERENCE_ROLE_GUEST
  int VCGetConferenceRole();
  
  /// Retrieves the current state of the conference.
  /// @return Either VC_CONFERENCE_STATE_DESTROYED or VC_CONFERENCE_STATE_CREATED
  int VCGetConferenceState();
  
  /// Retrieves the elapsed time of the conference.
  /// If the conference role is VC_CONFERENCE_ROLE_NONE, the elapsed time will always be 0.
  /// @return Elapsed time of the conference in ms.
  long VCGetConferenceElapsedTime();
  
  /// Retrieves the start timestamp of the conference.
  /// If the conference role is VC_CONFERENCE_ROLE_NONE, the start timestamp will always be 0.
  /// @return Start timestamp in ms since epoch.
  unsigned long long VCGetConferenceStartTimestamp();
  
  /// Retrieves the microphone's mute status for the conference.
  /// If the conference role is VC_CONFERENCE_ROLE_NONE, the mute status will always be false.
  /// @return true if the mic for the conference conference is currently muted, false otherwise.
  bool VCGetConferenceIsMute();
  
  /// Retrieves the call state for the specified partner.
  /// @param partner_id Fresvii user ID of the target partner.
  /// @param call_state Pointer to receive the call state of the partner.
  /// @return Error code.
  int VCGetCallState(const char* partner_id, int* call_state);
}

#endif  // #ifndef _VOICECHAT_LISTENERS_ENGINELISTENER_H__
