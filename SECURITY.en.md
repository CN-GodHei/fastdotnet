# Security Policy

## Supported Versions

The following versions of Fastdotnet are currently supported with security updates:

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: Yes |
| < 1.0   | :x: No             |

## Reporting a Vulnerability

The Fastdotnet team and community take software security seriously. If you discover a security vulnerability in the Fastdotnet framework or related plugins, please notify us immediately.

### Reporting Process

**Please do NOT report security vulnerabilities through public issue trackers.**

Instead, send detailed information to our security team via email:

📧 **Security Contact**: [yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)

Please include the following information in your email:

1. **Vulnerability Type**: Brief description of the vulnerability nature (e.g., SQL injection, XSS, privilege escalation, etc.)
2. **Affected Components**: Specify the affected modules, plugins, or features
3. **Reproduction Steps**: Provide detailed steps to reproduce the issue
4. **Potential Impact**: Describe the potential harm this vulnerability could cause
5. **Suggested Fix** (optional): If you have fix suggestions, feel free to provide them

### Response Commitment

We commit to:

- **Acknowledge receipt within 24 hours** of your report
- **Provide initial assessment and response plan within 72 hours**
- **Regular updates** on fix progress until the issue is resolved
- **Public acknowledgment**: After resolution, we will thank the discoverer in release notes (unless you request anonymity)

## Security Best Practices

### For Users

1. **Stay Updated**: Always use the latest version of Fastdotnet framework and plugins
2. **Secure Configuration**:
   - Change default passwords and administrator accounts
   - Enable HTTPS/TLS encrypted transmission
   - Configure appropriate firewall rules
3. **Dependency Management**: Regularly check and update third-party dependencies
4. **Monitor Logs**: Enable and regularly review application logs and security audit logs

### For Developers

1. **Input Validation**: Strictly validate and sanitize all user inputs
2. **Parameterized Queries**: Use parameterized queries to prevent SQL injection
3. **Authentication**:
   - Implement strong password policies
   - Enable multi-factor authentication (MFA)
   - Use secure session management
4. **Authorization Control**: Follow the principle of least privilege, implement fine-grained access control
5. **Data Protection**:
   - Encrypt sensitive data at rest
   - Avoid logging sensitive information
   - Implement data masking mechanisms
6. **Secure Coding**:
   - Follow OWASP Top 10 security guidelines
   - Conduct regular code reviews and security testing
   - Use static code analysis tools

## Known Security Issues

We publish known security issues and their status here. As of now:

- No publicly disclosed unpatched security vulnerabilities

Historical security issues will be documented in release notes after fixes.

## Security Update Policy

### Update Frequency

- **Critical Vulnerabilities**: Immediate patch release (typically within 7 days of confirmation)
- **Important Vulnerabilities**: Fixed in the next scheduled update
- **General Vulnerabilities**: Accumulated in regular releases

### Notification Channels

Security updates will be released through the following channels:

1. **GitHub Releases**: [https://github.com/CN-GodHei/fastdotnet/releases](https://github.com/CN-GodHei/fastdotnet/releases)
2. **Official Website Announcements**: [https://fastdotnet.top](https://fastdotnet.top)
3. **QQ Group**: 779454817
4. **Email Notifications**: Subscribe to our security announcement mailing list

## Third-Party Dependency Security

Fastdotnet depends on multiple third-party libraries. We:

- Continuously monitor dependency security advisories
- Regularly update dependencies to secure versions
- Use dependency scanning tools to detect known vulnerabilities
- Promptly release updates when critical vulnerabilities are found in dependencies

## Plugin Security

### Official Plugins

All officially released plugins undergo:

- Rigorous security code review
- Automated security scanning
- Penetration testing (for critical plugins)

### Third-Party Plugins

For community-developed plugins:

- We provide security development guidelines and best practices
- Encourage plugin developers to follow secure coding standards
- Marketplace performs basic security checks on listed plugins
- Users should carefully evaluate the security of third-party plugins

## Security Resources

Here are some useful security resources:

- **OWASP Top 10**: [https://owasp.org/www-project-top-ten/](https://owasp.org/www-project-top-ten/)
- **.NET Security Guide**: [https://docs.microsoft.com/en-us/dotnet/fundamentals/security/](https://docs.microsoft.com/en-us/dotnet/fundamentals/security/)
- **CWE/SANS Top 25**: [https://cwe.mitre.org/top25/](https://cwe.mitre.org/top25/)

## Contact Us

For any security-related questions or concerns, please contact:

- 📧 Email: [yunnanzuyuankeji@163.com](mailto:yunnanzuyuankeji@163.com)
- 💬 QQ Group: 779454817
- 🌐 Website: [https://fastdotnet.top](https://fastdotnet.top)

---

**Thank you for helping make Fastdotnet more secure!**