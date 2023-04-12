using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Paracord.Shared.Security.X509
{
    /// <summary>
    /// Builder for generating self-signed X509-certificates.
    /// </summary>
    public class X509CertificateBuilder
    {
        /// <summary>
        /// The subject for the certificate.
        /// This corresponds to the "Subject"-field in X509 certificates.
        /// </summary>
        public string Subject
        {
            get => string.Join(", ", this.Subjects.Select(v => $"{v.Key}={v.Value}"));
        }

        /// <summary>
        /// The individual parts of the Subject-line in x509 certificates.
        /// These are joined together to form the <c>Subject</c>-parameter.
        /// </summary>
        protected Dictionary<string, string> Subjects { get; set; }

        /// <summary>
        /// Set the oldest date/time when this certificate will be considered valid.
        /// Defaults to the current date/time, minus 1 hour.
        /// </summary>
        protected DateTimeOffset NotBefore { get; set; } = DateTimeOffset.Now.AddHours(-1);

        /// <summary>
        /// Set the expiration date for this certificate, when it will no longer be considered valid.
        /// Defaults to the current date/time, plus 2 years.
        /// </summary>
        protected DateTimeOffset NotAfter { get; set; } = DateTimeOffset.Now.AddYears(2);

        /// <summary>
        /// List of optional extensions to add to the certificate.
        /// </summary>
        protected List<X509Extension> Extensions { get; set; }

        /// <summary>
        /// Initialize a new <c>X509CertificateBuilder</c>.
        /// </summary>
        public X509CertificateBuilder()
        {
            this.Subjects = new Dictionary<string, string>();
            this.Extensions = new List<X509Extension>();
        }

        /// <summary>
        /// Build the certificate with the parameters specified and return it.
        /// </summary>
        /// <returns>The resulting <see cref="X509Certificate2" />.</returns>
        public X509Certificate2 Build()
        {
            using RSA rsa = RSA.Create();

            CertificateRequest request = new CertificateRequest(
                this.Subject, rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            foreach(var extension in this.Extensions)
            {
                request.CertificateExtensions.Add(extension);
            }

            return request.CreateSelfSigned(this.NotBefore, this.NotAfter);
        }

        /// <summary>
        /// Clear the current subject to allow for new subject-components.
        /// </summary>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder ClearSubject()
        {
            this.Subjects.Clear();
            return this;
        }

        /// <summary>
        /// Add the specified subject-component(s) to the current subject.
        /// </summary>
        /// <remarks>
        /// The input is a key-value pair, such as <c>["O", "Organization, Inc"]</c>, <c>["C", "DA"]</c>, etc.
        /// </remarks>
        /// <param name="subject">The subject-component to add to the current subject.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder AddSubject(KeyValuePair<string, string> subject)
        {
            if(this.Subjects.ContainsKey(subject.Key))
            {
                this.Subjects[subject.Key] = subject.Value;
            }
            else
            {
                this.Subjects.Add(subject.Key, subject.Value);
            }

            return this;
        }

        /// <inheritdoc cref="X509CertificateBuilder.AddSubject(KeyValuePair{string, string})" />
        /// <param name="key">The key for the subject-component.</param>
        /// <param name="value">The value for the subject-component.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder AddSubject(string key, string value)
            => this.AddSubject(new KeyValuePair<string, string>(key, value));

        /// <summary>
        /// Set the country of the certificate.
        /// </summary>
        /// <param name="country">The new country of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetCountry(string country)
            => this.AddSubject("C", country);

        /// <summary>
        /// Set the state of the certificate.
        /// </summary>
        /// <param name="state">The new state of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetState(string state)
            => this.AddSubject("ST", state);

        /// <summary>
        /// Set the city of the certificate.
        /// </summary>
        /// <param name="city">The new city of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetCity(string city)
            => this.AddSubject("L", city);

        /// <summary>
        /// Set the organization of the certificate.
        /// </summary>
        /// <param name="organization">The new organization of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetOrganization(string organization)
            => this.AddSubject("O", organization);

        /// <summary>
        /// Set the common-name of the certificate.
        /// </summary>
        /// <param name="commonName">The new common-name of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetCommonName(string commonName)
            => this.AddSubject("CN", commonName);

        /// <summary>
        /// Clear the current extensions to allow for new ones.
        /// </summary>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder ClearExtensions()
        {
            this.Extensions.Clear();
            return this;
        }

        /// <summary>
        /// Add the specified <see cref="X509Extension" />-extension to the certificate.
        /// </summary>
        /// <param name="extension">The <see cref="X509Extension" />-extension to add to the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder AddExtension(X509Extension extension)
        {
            this.Extensions.Add(extension);
            return this;
        }

        /// <summary>
        /// Set the oldest date/time when this certificate will be considered valid.
        /// </summary>
        /// <param name="notBefore">The new <c>NotBefore</c>-parameter of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetNotBefore(DateTimeOffset notBefore)
        {
            this.NotBefore = notBefore;
            return this;
        }

        /// <summary>
        /// Set the expiration date for this certificate, when it will no longer be considered valid.
        /// </summary>
        /// <param name="notAfter">The new <c>NotAfter</c>-parameter of the certificate.</param>
        /// <returns>The current <see cref="X509CertificateBuilder" />, to allow for chaining commands.</returns>
        public X509CertificateBuilder SetNotAfter(DateTimeOffset notAfter)
        {
            this.NotAfter = notAfter;
            return this;
        }
    }
}