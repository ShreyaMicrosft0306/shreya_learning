# Operations, Security, and Communication

## Observability

Instrument the user journey, not only host health:

- **Metrics:** request rate, error rate, latency percentiles, queue depth/age, saturation, cache hit rate, and business outcomes.
- **Logs:** structured, contextual events with request/correlation IDs; do not log secrets or unnecessary personal data.
- **Traces:** causal spans across services and asynchronous work.
- **Alerts:** page on actionable symptoms tied to user impact, not every transient fluctuation.

Define service-level indicators (what is measured), objectives (the target), and an error budget that informs release risk. A queue can be “up” while its oldest message is hours behind; monitor age and throughput.

## Security and privacy

Start from identity and trust boundaries. Authenticate users and services, authorize every resource access, and enforce tenant isolation at the data boundary. Use least privilege, secret rotation, encryption in transit and at rest, and secure defaults.

Consider abuse and adversarial traffic: rate limits, quotas, spam/bot controls, replay protection, audit trails, and anomaly detection. Minimize retained sensitive data and define deletion/retention behavior. Protect payment details by using a compliant provider rather than storing raw card data.

## Availability and recovery

Describe redundancy across failure domains, health checks, failover, backup/restore, and recovery objectives. A backup is useful only if restore has been tested. Multi-region deployment may improve recovery or latency but adds replication, routing, and conflict-resolution complexity.

Use safe rollout strategies such as gradual traffic shifts, backward-compatible schema changes, feature flags, and rollback plans. Design event schemas and APIs for version compatibility.

## Communicating a design

Use a clear diagram with named responsibilities and arrows labeled by protocol or event. For each new component, state:

1. the requirement it addresses;
2. the data it owns;
3. its failure behavior;
4. the cost or complexity it introduces.

When uncertain, present a baseline and one alternative. Explain what workload or requirement would make you switch. Keep security, privacy, and operability in the design rather than adding them as afterthoughts.

## Final review checklist

- Can I trace the main request from client to durable state and back?
- Are authorization and tenant boundaries clear?
- What happens if a dependency is slow, down, or returns after a retry?
- Can operators detect, diagnose, and recover the important failures?
- Which data is authoritative, derived, cached, or disposable?
- Did I state the strongest consistency guarantees and why they are needed?
- Did I identify at least one cost or complexity trade-off?

