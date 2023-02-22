export default abstract class StatusCode {
  static readonly OK = 200;

  static readonly CREATED = 201;

  static readonly NO_CONTENT = 204;

  static readonly MOVED_PERMANENTLY = 301;

  static readonly MOVED_TEMPORARILY = 302;

  static readonly TEMPORARY_REDIRECT = 307;

  static readonly PERMANENT_REDIRECT = 308;

  static readonly BAD_REQUEST = 400;

  static readonly UNAUTHORIZED = 401;

  static readonly FORBIDDEN = 403;

  static readonly NOT_FOUND = 404;

  static readonly METHOD_NOT_ALLOWED = 405;

  static readonly CONFLICT = 409;

  static readonly INTERNAL_SERVER_ERROR = 500;
}
