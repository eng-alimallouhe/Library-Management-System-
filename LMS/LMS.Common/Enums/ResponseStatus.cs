namespace LMS.Common.Enums
{
    public enum ResponseStatus
    {
        //Authentication StatusKey:
        //AUTHENTICATION.
        WEAK_PASSWORD,
        ALREADY_EXISTING_ACCOUNT,
        ACTIVATION_FAILED,
        ACCOUNT_NOT_FOUND,
        AUTHENTICATION_SUCCESS,
        AUTHENTICATION_FAILED,
        INVALID_TOKEN,
        ACTIVATION_SUCCESS,
        EXPIRED_SESSION, // when the regenerate for  the refresh token be failed 
        FILED_ATTEMPT,
        MAX_ATTEMPT,
        UNVERIFIED_ACCOUNT,
        LOCKED_ACCOUNT,
        MAX_LOGIN_ATTEMPTS,
        USER_NAME_AVALIABLE,
        USER_NAME_UNAVALIABLE,
        EXISTING_ACCOUNT,
        TWO_FACTOR_REQUIRED,


        //OTPCODE:
        INDEFINITE_TIME_PERIOD,
        SUCCESSS_CODE_SEND,
        EXPIRED_CODE,
        INVALID_CODE,
        EMAIL_SEND_FAILED,





        // Common ResponseStatusKey for errors:
        UNKNOWN_ERROR,
        DELETE_COMPLETED,
        TASK_COMPLETED,
        ADD_COMPLETED,
        UPDATE_COMPLETED,
        UPLOAD_FAILED,
        UPLOAD_SUCCESS,
        SOURCE_NOT_FOUND,
        UNABLE_DELETE_NOT_FOUNDED_ELEMENT,
        UNABLE_UPDATE_NOT_FOUNDED_ELEMENT,
        AUTHORIZATION_REQUIERD,
        RECORD_STATUS_CONNOT_BE_MODIFIED,
        STATUS_ALREADY_SET,
        UNABLE_DELETE_ELEMENT,
        RECORD_NOT_FOUND,
        ALREADE_EXIST_RECORD,



        //HR 
        //DEPARTMENTS 
        DEPARTMENT_NOT_FOUNDED,


        //HR
        //EMPLOYEES
        EMPLOYEE_NOT_FOUNDED,
        EXISTING_APPOINTMENT,
        FACE_EXTRACTION_FAILED, //when we try to extract the face vector from the provided iamge 


        //HR
        //LEAVES:
        BALANCE_NOT_FOUNDED,
        BALANCE_NOT_ENOUGH,
        INVALID_DATE_RANGE,


        //HR
        //ATTENDANCES
        FACE_NOT_SAME,
        CANNAT_CHECK_IN,
        CANNAT_CHECK_OUT,
        CHECKING_SUCCESS,
        ALREADY_CHECKINED_OUT,
        ALREADY_CHECKINED_IN
    }
}
