package com.capstone.Arogya.model;

import jakarta.persistence.*;
import lombok.*;

import java.util.HashSet;
import java.util.Set;

@Entity
@Table(name = "users")
@Data               
@NoArgsConstructor  
@AllArgsConstructor 
@Builder           
public class User {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, unique = true)
    private String username;

    @Column(nullable = false)
    private String password;

    private String email;

    private String firstName;

    private String lastName;

    @Builder.Default
    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(
        name = "user_roles",
        joinColumns = @JoinColumn(name = "user_id"),
        inverseJoinColumns = @JoinColumn(name = "role_id")
    )
    private Set<Role> roles = new HashSet<>();

    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(
        name = "user_culture_religion",
        joinColumns = @JoinColumn(name = "user_id"),
        inverseJoinColumns = @JoinColumn(name = "culture_religion_id")
    )
    private Set<CultureReligion> cultureAndReligion = new HashSet<>();

    
    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(
        name = "user_social_political",
        joinColumns = @JoinColumn(name = "user_id"),
        inverseJoinColumns = @JoinColumn(name = "social_political_id")
    )
    private Set<SocialPolitical> socialAndPolitical = new HashSet<>();

    
    @ManyToMany(fetch = FetchType.LAZY)
    @JoinTable(
        name = "user_health_wellness",
        joinColumns = @JoinColumn(name = "user_id"),
        inverseJoinColumns = @JoinColumn(name = "health_wellness_id")
    )
    private Set<HealthWellness> healthAndWellness = new HashSet<>();

    @OneToMany(mappedBy = "user", 
        cascade = CascadeType.ALL,
        orphanRemoval = true, 
        fetch = FetchType.LAZY)
    private Set<Goal> goals = new HashSet<>();
}