//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (http://www.swig.org).
// Version 4.0.2
//
// Do not make changes to this file unless you know what you are doing--modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class vx_req_aux_create_account_t : global::System.IDisposable {
  private global::System.Runtime.InteropServices.HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal vx_req_aux_create_account_t(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }

  internal static global::System.Runtime.InteropServices.HandleRef getCPtr(vx_req_aux_create_account_t obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }

  ~vx_req_aux_create_account_t() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          VivoxCoreInstancePINVOKE.delete_vx_req_aux_create_account_t(swigCPtr);
        }
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }

        public static implicit operator vx_req_base_t(vx_req_aux_create_account_t t) {
            return t.base_;
        }
        
  public vx_req_base_t base_ {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_base__set(swigCPtr, vx_req_base_t.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_base__get(swigCPtr);
      vx_req_base_t ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_req_base_t(cPtr, false);
      return ret;
    } 
  }

  public vx_generic_credentials credentials {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_credentials_set(swigCPtr, vx_generic_credentials.getCPtr(value));
    } 
    get {
      global::System.IntPtr cPtr = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_credentials_get(swigCPtr);
      vx_generic_credentials ret = (cPtr == global::System.IntPtr.Zero) ? null : new vx_generic_credentials(cPtr, false);
      return ret;
    } 
  }

  public string user_name {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_user_name_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_user_name_get(swigCPtr);
      return ret;
    } 
  }

  public string password {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_password_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_password_get(swigCPtr);
      return ret;
    } 
  }

  public string email {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_email_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_email_get(swigCPtr);
      return ret;
    } 
  }

  public string number {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_number_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_number_get(swigCPtr);
      return ret;
    } 
  }

  public string displayname {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_displayname_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_displayname_get(swigCPtr);
      return ret;
    } 
  }

  public string firstname {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_firstname_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_firstname_get(swigCPtr);
      return ret;
    } 
  }

  public string lastname {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_lastname_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_lastname_get(swigCPtr);
      return ret;
    } 
  }

  public string phone {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_phone_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_phone_get(swigCPtr);
      return ret;
    } 
  }

  public string lang {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_lang_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_lang_get(swigCPtr);
      return ret;
    } 
  }

  public string age {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_age_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_age_get(swigCPtr);
      return ret;
    } 
  }

  public string gender {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_gender_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_gender_get(swigCPtr);
      return ret;
    } 
  }

  public string timezone {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_timezone_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_timezone_get(swigCPtr);
      return ret;
    } 
  }

  public string ext_profile {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_ext_profile_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_ext_profile_get(swigCPtr);
      return ret;
    } 
  }

  public string ext_id {
    set {
      VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_ext_id_set(swigCPtr, value);
    } 
    get {
      string ret = VivoxCoreInstancePINVOKE.vx_req_aux_create_account_t_ext_id_get(swigCPtr);
      return ret;
    } 
  }

  public vx_req_aux_create_account_t() : this(VivoxCoreInstancePINVOKE.new_vx_req_aux_create_account_t(), true) {
  }

}
